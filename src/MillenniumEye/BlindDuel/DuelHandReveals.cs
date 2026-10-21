using System.Collections.Generic;
using Il2CppYgomGame.Duel;

namespace MillenniumEye;

// Correlate only the same native unique ID within the current action context.
// Retain public IDs, never hidden identities or a snapshot of an entire hand.
internal static class DuelHandReveals
{
    private sealed class Entry
    {
        public int Owner, Position, CardId, FromPosition;
        public bool Added, Announced;
    }

    private static readonly Dictionary<int, Entry> Entries = new Dictionary<int, Entry>();
    public static void Reset() => Entries.Clear();

    public static void Move(int uniqueId, Engine.CardStatus from, Engine.CardStatus to, int publicId)
    {
        if (uniqueId <= 0 || !DuelEventReader.TryGetPerspective(out int me)) return;
        if (from.Player == to.Player && from.Position == to.Position) return;
        Entries.TryGetValue(uniqueId, out var previous);
        Entries.Remove(uniqueId); // Every move invalidates the previous location.
        if (to.Player == me || to.Player < 0 || to.Player > 1 || to.Position != Engine.PosHand) return;
        if (publicId <= 0 && previous != null && !previous.Added &&
            ((previous.Owner == from.Player && previous.Position == from.Position) ||
             (previous.Owner == to.Player && previous.Position == to.Position)))
            publicId = previous.CardId;
        var entry = new Entry { Owner = to.Player, Position = to.Position, CardId = publicId,
            FromPosition = from.Position, Added = true };
        Entries[uniqueId] = entry;
        if (publicId > 0) AnnounceAddition(entry);
    }

    public static void Reveal(int uniqueId, LogDataSide card)
    {
        if (uniqueId <= 0 || !DuelEventReader.TryGetPerspective(out int me) || !card.isCardDataShow ||
            card.cardid <= 0 || card.owner == me || card.owner < 0 || card.owner > 1) return;
        if (Entries.TryGetValue(uniqueId, out var entry) && entry.Owner == card.owner && entry.Position == card.position)
        {
            if (entry.CardId == card.cardid && entry.Announced) return;
            entry.CardId = card.cardid;
            if (entry.Added) { AnnounceAddition(entry); return; }
        }
        else
        {
            entry = new Entry { Owner = card.owner, Position = card.position, CardId = card.cardid };
            Entries[uniqueId] = entry;
        }
        entry.Announced = true;
        DuelEventReader.Announce("Opponent reveals " + Name(card.cardid) + " in " + DuelEventReader.Location(card.position), card.cardid);
    }

    private static string Name(int publicId)
    {
        string name = DuelLogHelper.ResolveCardName(publicId);
        return string.IsNullOrWhiteSpace(name) || name == "unknown" ? "a card (name unavailable)" : name;
    }

    private static void AnnounceAddition(Entry entry)
    {
        entry.Announced = true;
        DuelEventReader.Announce("Opponent added " + Name(entry.CardId) + " from " +
            DuelEventReader.Location(entry.FromPosition) + " to hand", entry.CardId);
    }
}
