# Validation and native research

Initial game: Master Duel 2.8.0, Steam build 24462996, Unity 6000.0.61f1,
IL2CPP metadata 31.1. Supported GameAssembly SHA-256:
`CA24A284A86DC394F6B0CBEAAD455D29E5F6B95FC8DB8FC42812B29DE3C7620C`.

## Native evidence

Selective current-image Ghidra inspection corroborated these bindings:

| Native seam | RVA | Use |
| --- | --- | --- |
| Engine.GetCardNum | 0x100E330 | Visible zone counts |
| Engine.GetCardBasicValByUniqueID | 0x100D780 | Current monster values |
| CardInfo.SetDescriptionArea | 0x1052510 | Refresh the visible detail panel |
| EngineApiUtil.IsCardKnown, three arguments | 0x58CC50 | Native public identity gate |
| EngineApiUtil.IsCardKnown, four arguments | 0x58CA50 | Visibility implementation |
| DuelLogController.PushBuff | 0x456FD0 | Committed public action/card records |
| ActivateConfirmToggle.OnClickToggle | 0x1044E80 | Player-requested native toggle cycle |
| ActivateConfirmToggle.SetIcon | 0x1045430 | Visible mode announcements |
| ActivateConfirmToggle.SetManualType | 0x10454A0 | Native setting and interaction constraints |

These RVAs are build-specific research evidence, not hardcoded memory offsets
used by the mod. Harmony uses the current generated bindings.
The toggle's native enum is Default / Off / On; Default is spoken as Auto.
The public search policy requires the native action and card visibility flags,
an opponent-owned deck-to-hand move, and a positive public card ID.

## Confirmed checks

- Full renamed mod build: zero warnings/errors.
- 21 offline reader checks passed: both stats and zeros, Link/Trap cases,
  public-versus-hidden search policy, copied/reset detail buffers and native
  toggle boundary behavior.
- 72 patch targets resolved against the regenerated installed bindings.
- 25 installation regression checks use temporary game/payload fixtures and
  cover legacy replacement, fresh removal, preservation of saves, reinstallation,
  corrupt payloads, duplicate readers, unsupported builds/loaders, unsafe paths,
  unknown readers, renamed duplicate readers, rollback after a controlled copy
  failure and retrying an interrupted restore. The host process inventory is a
  boundary double so fixture tests do not close a player's running game.
- The exact Millennium Eye 0.1.0 DLL loaded through Steam at Master Duel 2.8.0,
  detected NVDA and registered 20 screen handlers without a failed Harmony
  patch. The player confirmed audible title-screen and card/menu speech on
  October 13, 2026. The publication audit records the exact deployed hashes.

## Remaining player validation

Actual duels, multiple simultaneous card/effect transitions, controller input,
public search announcements during a duel, draw/standby timing and controller
play have not been confirmed. The offline toggle tests use boundary doubles;
neither those tests nor startup/menu speech prove live duel behavior.
Broader inherited deck editor, shop, tutorial and menu coverage also needs
player testing. The beta label reflects these limits.

The earlier startup check observed the game rewriting one opaque LocalSave
file. Its semantic meaning is unknown. Deployment itself replaced only owned
mod files. Current saves/settings are backed up again before the release's
startup check; any native game writes are reported without calling them
byte-for-byte save preservation.
