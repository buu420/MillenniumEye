# Changelog

## 0.1.1 — 2026-10-21 (beta)

- Announce opponent attacks with the attacking monster and the attacked card's
  field location. Direct attacks use the native life-target marker.
- Announce effect targets on the field, in the graveyard and in other piles.
  Non-target choices use "Selected" so they are not mistaken for targeting.
- Read public hand additions at the move/reveal event, covering reveals before
  and after the move and multiple added cards. Hidden identities stay unnamed.
- Queue automatic response/focus speech behind activations and their targets.
  Deliberate keyboard or controller navigation can interrupt normally.
- Replace the restricted attack-selection callback and buffered-search-only
  reader. Upgrade the recognized 0.1.0 installation in place with rollback.

Verified offline against the supported game bindings. Live duel timing,
audible event speech and controller play still need player validation.

## 0.1.0 — 2026-10-13 (beta)

First Millennium Eye release, continuing RealAmethyst's BlindDuel. This version
sequence starts at 0.1.0 and does not replace the original project's version history.

- Opponent hand, deck and extra-deck counts, individually and together.
- Both current monster stats independent of battle position; Link and Trap
  Monster handling follow the visible game data.
- Fresh details for selections, public activations and deferred field focus.
- Publicly revealed opponent deck-to-hand search announcements.
- F8 native activation-toggle cycling and Ctrl+F8 mode reading, with disabled
  control guidance and spoken controller-driven mode changes.
- Current-game confirmation and home submenu hooks; removed a failing legacy
  Left Alt object-search path.
- Portable source build, regression checks, license attribution, installation
  backups, conflict detection and AMM packaging.

Initial compatibility: Windows x64 Master Duel 2.8.0 / Steam build 24462996,
MelonLoader 0.7.2. The player confirmed audible startup and card/menu speech.
Live duel behavior, controller input and activation timing remain unconfirmed;
the release is a beta.
