using System;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

// The native move logger (0x44E470) uses these exact status snapshots and
// IsCardKnown(status) gates before resolving a unique ID to a card identity.
// Capture before mutable engine state advances; publish after the native handler.
[HarmonyPatch(typeof(DuelLogController), "AddCardMoveLog")]
internal static class PatchPublicCardMoveLog
{
    internal sealed class MoveSnapshot
    {
        public int UniqueId, PublicId;
        public Engine.CardStatus From, To;
        public bool Valid;
        public MoveSnapshot Previous;
    }

    [ThreadStatic] private static MoveSnapshot _activeMove;

    [HarmonyPrefix]
    private static void Prefix(int param1, int param2, int param3, out MoveSnapshot __state)
    {
        __state = new MoveSnapshot { Previous = _activeMove };
        _activeMove = __state;
        if (!DuelEventReader.TryGetPerspective(out int me)) return;
        try
        {
            var from = new Engine.CardStatus(param2);
            var to = new Engine.CardStatus(param3);
            int uid = EngineApiUtil.BreakLong(param1).Item1;
            if (uid <= 0) return;
            __state.UniqueId = uid;
            __state.From = from;
            __state.To = to;
            __state.Valid = true;
            int publicId = 0;
            if (to.Player != me && to.Position == Engine.PosHand &&
                (EngineApiUtil.IsCardKnown(from) || EngineApiUtil.IsCardKnown(to)))
                publicId = Engine.GetCardIDByUniqueID(uid);
            __state.PublicId = publicId;
        }
        catch (Exception ex) { DuelHandReveals.Reset(); Log.Write("[HandEvent] Move snapshot failed: " + ex.Message); }
    }

    [HarmonyPostfix]
    private static void Postfix(MoveSnapshot __state)
    {
        if (__state == null) return;
        try { if (__state.Valid) DuelHandReveals.Move(__state.UniqueId, __state.From, __state.To, __state.PublicId); }
        catch (Exception ex) { Log.Write("[HandEvent] Move read failed: " + ex.Message); }
        finally { _activeMove = __state.Previous; }
    }

    [HarmonyFinalizer]
    private static void Finalizer(MoveSnapshot __state)
    {
        if (__state != null && ReferenceEquals(_activeMove, __state)) _activeMove = __state.Previous;
    }

    internal static void CapturePublicSearch(ShowActionData action)
    {
        var move = _activeMove;
        if (move == null || !move.Valid || !action.datac.isActDataShow ||
            action.datac.acttype != LOGACTIONTYPE.ACTION_SEARCH ||
            !action.datal.isCardDataShow || !action.datar.isCardDataShow || action.datar.cardid <= 0) return;
        if (action.datal.owner == move.From.Player && action.datal.position == move.From.Position &&
            action.datar.owner == move.To.Player && action.datar.position == move.To.Position)
            move.PublicId = action.datar.cardid;
    }
}

// ACTION_SEARCH is explicitly public in AddCardMoveLog even when neither
// status passes IsCardKnown. Capture that sanitized record, not a raw engine ID.
[HarmonyPatch(typeof(DuelLogController), "BuffMoveData")]
internal static class PatchPublicSearchRecord
{
    [HarmonyPrefix]
    private static void Prefix(ShowActionData actionData)
    {
        try { PatchPublicCardMoveLog.CapturePublicSearch(actionData); }
        catch (Exception ex) { Log.Write("[HandEvent] Public search record failed: " + ex.Message); }
    }
}

// AddHandOpenLog 0x4500B0 creates one ACTION_HANDOPEN record in a separate
// queue. Positive modes show the card; zero closes it, negative modes annotate
// costs and are skipped by EffectTaskHandOpen. Do not read its full-hand arrays.
[HarmonyPatch(typeof(DuelLogController), "AddHandOpenLog")]
internal static class PatchPublicHandOpenLog
{
    [HarmonyPrefix]
    private static void Prefix(DuelLogController __instance, int param2, out int __state)
    {
        __state = -1;
        if (param2 <= 0 || !DuelEventReader.TryGetPerspective(out _)) return;
        try { __state = __instance.m_HandOpenDataQueue?.Count ?? -1; }
        catch (Exception ex) { Log.Write("[HandEvent] Reveal snapshot failed: " + ex.Message); }
    }

    [HarmonyPostfix]
    private static void Postfix(DuelLogController __instance, int param2, int param3, int __state)
    {
        if (__state < 0 || param2 <= 0 || param3 <= 0 || !DuelEventReader.TryGetPerspective(out _)) return;
        try
        {
            var queue = __instance.m_HandOpenDataQueue;
            if (queue == null || queue.Count <= __state) return;
            var items = queue.ToArray();
            for (int i = __state; i < items.Length; i++)
            {
                var action = items[i].Key;
                if (action.datac.isActDataShow && action.datac.acttype == LOGACTIONTYPE.ACTION_HANDOPEN)
                    DuelHandReveals.Reveal(param3, action.datal);
            }
        }
        catch (Exception ex) { Log.Write("[HandEvent] Reveal read failed: " + ex.Message); }
    }
}

// RunEffectImpl is the common view dispatch, before its log/task callbacks.
// A new activation, phase or shuffled deck cannot inherit an earlier reveal.
[HarmonyPatch(typeof(DuelClient), "RunEffectImpl")]
internal static class PatchPublicRevealBoundary
{
    [HarmonyPrefix]
    private static void Prefix(int id)
    {
        var view = (Engine.ViewType)id;
        if (view == Engine.ViewType.CardHappen || view == Engine.ViewType.PhaseChange ||
            view == Engine.ViewType.TurnChange || view == Engine.ViewType.DeckShuffle ||
            view == Engine.ViewType.HandShuffle || view == Engine.ViewType.DeckReset)
            DuelHandReveals.Reset();
    }
}
