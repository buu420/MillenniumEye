using System;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

internal static class DuelEventReader
{
    public static bool TryGetPerspective(out int player)
    {
        player = -1;
        if (!NavigationState.IsInDuel || DuelState.IsShowingResult) return false;
        try
        {
            var initializer = DuelClient.instance?.engineInitializer;
            if (initializer == null) return false;
            player = initializer.myPlayerNum;
            return player == 0 || player == 1;
        }
        catch (Exception ex) { Log.Write("[DuelEvent] Perspective unavailable: " + ex.Message); return false; }
    }

    public static int CaptureActionCount(DuelLogController log)
    {
        if (!TryGetPerspective(out _)) return -1;
        try { return log.m_DataList_ShowAction?.Count ?? -1; }
        catch (Exception ex) { Log.Write("[DuelEvent] Snapshot failed: " + ex.Message); return -1; }
    }

    public static void ReadNewActions(DuelLogController log, int start, Action<ShowActionData> read)
    {
        if (start < 0 || !TryGetPerspective(out _)) return;
        try
        {
            var actions = log.m_DataList_ShowAction;
            if (actions == null) return;
            int end = actions.Count;
            for (int i = start; i < end; i++) read(actions[i]);
        }
        catch (Exception ex) { Log.Write("[DuelEvent] Read failed: " + ex.Message); }
    }

    private static string Name(LogDataSide card)
    {
        // These records have already passed the native IsCardKnown gate.
        // Unknown native identities are zero. Never look them up in the engine.
        if (!card.isCardDataShow || card.cardid <= 0) return "card";
        string name = DuelLogHelper.ResolveCardName(card.cardid);
        return string.IsNullOrWhiteSpace(name) || name == "unknown" ? "card" : name;
    }

    public static string Location(int position)
    {
        if (position >= 0 && position <= 4) return "Monster Zone " + (position + 1);
        if (position == 5 || position == 6) return "Extra Monster Zone " + (position - 4);
        if (position >= 7 && position <= 11) return "Spell/Trap Zone " + (position - 6);
        if (position == Engine.PosField) return "Field Zone";
        if (position == Engine.PosHand) return "hand";
        if (position == Engine.PosExtra) return "Extra Deck";
        if (position == Engine.PosDeck) return "deck";
        if (position == Engine.PosGrave) return "graveyard";
        if (position == Engine.PosExclude) return "banished cards";
        return "location unavailable";
    }

    private static string Describe(LogDataSide card, int me)
    {
        string owner = card.owner == me ? "your " : "opponent's ";
        return owner + Name(card) + ", " + Location(card.position);
    }

    public static void ReadAttack(ShowActionData action, int nativeTargetPosition)
    {
        if (!TryGetPerspective(out int me) || nativeTargetPosition < 0 ||
            !action.datac.isActDataShow || action.datac.acttype != LOGACTIONTYPE.ACTION_ATTACK ||
            !action.datal.isCardDataShow || action.datal.owner == me ||
            action.datal.owner < 0 || action.datal.owner > 1) return;
        string attacker = Name(action.datal) + ", " + Location(action.datal.position);
        string message;
        if (nativeTargetPosition > 6 && action.datar.isIconDataShow)
            message = "Opponent attacks directly with " + attacker;
        else if (nativeTargetPosition <= 6 && action.datar.isCardDataShow)
            message = "Opponent attacks " + Describe(action.datar, me) + " with " + attacker;
        else
            message = "Opponent attacks with " + attacker + ". Target location: " + Location(nativeTargetPosition);
        Announce(message, action.datal.cardid);
    }

    public static void ReadLockon(ShowActionData action, int lockonType)
    {
        if (!TryGetPerspective(out int me) || (lockonType != 0 && lockonType != 1) ||
            !action.datac.isActDataShow || action.datac.acttype != LOGACTIONTYPE.ACTION_LOCKON ||
            !action.datal.isCardDataShow || action.datal.owner < 0 || action.datal.owner > 1) return;
        // Type 1 uses the same ACTION_LOCKON but represents a non-target choice.
        string label = lockonType == 0 ? "Target: " : "Selected: ";
        Announce(label + Describe(action.datal, me), action.datal.cardid);
    }

    public static void Announce(string message, int publicCardId)
    {
        CardReader.SetPublicDuelDetails(publicCardId);
        FieldFocusHandler.ClearLastFocus();
        DuelState.LastQueuedButtonText = null;
        DuelState.MessageJustAnnounced = true;
        Log.Write("[DuelEvent] " + message);
        Speech.SayDuelEvent(message);
    }
}
