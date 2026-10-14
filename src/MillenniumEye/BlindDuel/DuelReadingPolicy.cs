// Derived from BlindDuel by RealAmethyst and contributors; LGPL-3.0.
// Recovered and modified for Millennium Eye by buu420, 2026-10-13.
namespace MillenniumEye;

public static class DuelReadingPolicy
{
    public static string ZoneCount(string label, int? count) => count.HasValue && count.Value >= 0
        ? $"{label}, {count.Value} cards" : label + ", count unavailable";
    public static bool CanAnnounceSearch(bool actionShown, bool cardShown, bool isSearch,
        bool fromDeck, bool toHand, bool opponent, bool sameOwner, int cardId) =>
        actionShown && cardShown && isSearch && fromDeck && toHand && opponent && sameOwner && cardId > 0;
}
