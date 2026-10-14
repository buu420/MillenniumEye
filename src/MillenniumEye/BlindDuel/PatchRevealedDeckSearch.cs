// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
using System;
using System.Collections.Generic;
using HarmonyLib;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

// Ghidra RVA 0x456FD0 commits m_LogDataBuff to the native duel log.
// Copy before that commit clears the buffer; announce only after it succeeds.
[HarmonyPatch(typeof(DuelLogController), "PushBuff")]
internal static class PatchRevealedDeckSearch
{
    [HarmonyPrefix]
    private static void Prefix(DuelLogController __instance, out List<ShowActionData> __state)
    {
        __state = null;
        if (!NavigationState.IsInDuel || DuelState.IsShowingResult) return;
        try
        {
            var buffer = __instance.m_LogDataBuff;
            if (buffer == null || buffer.Count == 0) return;
            __state = new List<ShowActionData>();
            for (int i = 0; i < buffer.Count; i++) __state.Add(buffer[i]);
        }
        catch (Exception ex) { Log.Write("[RevealedSearch] Snapshot failed: " + ex.Message); }
    }

    [HarmonyPostfix]
    private static void Postfix(List<ShowActionData> __state)
    {
        if (__state == null) return;
        foreach (var action in __state)
        {
            try
            {
                var from = action.datal;
                var to = action.datar;
                if (!DuelReadingPolicy.CanAnnounceSearch(action.datac.isActDataShow,
                    to.isCardDataShow, action.datac.acttype == LOGACTIONTYPE.ACTION_SEARCH,
                    from.position == Engine.PosDeck, to.position == Engine.PosHand,
                    !DuelState.IsMyPlayer(to.owner), from.owner == to.owner, to.cardid)) continue;
                string name = DuelLogHelper.ResolveCardName(to.cardid);
                if (string.IsNullOrWhiteSpace(name) || name == "unknown") continue;
                CardReader.SetPublicDuelDetails(to.cardid);
                FieldFocusHandler.ClearLastFocus();
                DuelState.LastQueuedButtonText = null;
                DuelState.MessageJustAnnounced = true;
                Log.Write($"[RevealedSearch] Visible native log: cardid={to.cardid}, owner={to.owner}");
                Speech.SayQueued("Opponent added " + name + " from deck to hand");
            }
            catch (Exception ex) { Log.Write("[RevealedSearch] Read failed: " + ex.Message); }
        }
    }
}
