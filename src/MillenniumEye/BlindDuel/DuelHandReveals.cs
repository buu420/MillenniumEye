using System.Collections.Generic;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

// Deduplicate identities already announced by a public move/reveal. Hidden
// moves never retain an origin or acquire an identity from a later reveal.
internal static class DuelHandReveals
{
    private sealed class Entry
    {
        public int Owner, Position, CardId;
    }

    private static readonly Dictionary<int, Entry> Entries = new Dictionary<int, Entry>();
    public static void Reset() => Entries.Clear();

    public static void Move(int uniqueId, Engine.CardStatus from, Engine.CardStatus to, int publicId)
    {
        if (uniqueId <= 0 || !DuelEventReader.TryGetPerspective(out int me)) return;
        if (from.Player == to.Player && from.Position == to.Position) return;
        Entries.Remove(uniqueId); // Every move invalidates the previous location.
        if (publicId <= 0 || to.Player == me || to.Player < 0 || to.Player > 1 || to.Position != Engine.PosHand) return;
        Entries[uniqueId] = new Entry { Owner = to.Player, Position = to.Position, CardId = publicId };
        string source = (from.Player == me ? "your " : "") + DuelEventReader.Location(from.Position);
        DuelEventReader.Announce("Opponent added " + Name(publicId) + " from " +
            source + " to hand", publicId);
    }

    public static void Reveal(int uniqueId, LogDataSide card)
    {
        if (uniqueId <= 0 || !DuelEventReader.TryGetPerspective(out int me) || !card.isCardDataShow ||
            card.cardid <= 0 || card.position != Engine.PosHand ||
            card.owner == me || card.owner < 0 || card.owner > 1) return;
        if (Entries.TryGetValue(uniqueId, out var entry) && entry.Owner == card.owner &&
            entry.Position == card.position && entry.CardId == card.cardid) return;
        Entries[uniqueId] = new Entry { Owner = card.owner, Position = card.position, CardId = card.cardid };
        DuelEventReader.Announce("Opponent reveals " + Name(card.cardid) + " in " + DuelEventReader.Location(card.position), card.cardid);
    }

    private static string Name(int publicId)
    {
        string name = DuelLogHelper.ResolveCardName(publicId);
        return string.IsNullOrWhiteSpace(name) || name == "unknown" ? "a card (name unavailable)" : name;
    }

}
