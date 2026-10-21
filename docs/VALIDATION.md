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
| DuelClient.RunEffectImpl | 0xFD6640 | Common native view dispatch and reveal-context boundaries |
| DuelLogController.AddBattleAttackLog | 0x44B890 | Attack source and native card/life target |
| DuelLogController.AddCardLockonLog | 0x44DFC0 | Public target/selection records, including piles |
| DuelLogController.AddCardMoveLog | 0x44E470 | Immutable source/destination status and native identity gate |
| DuelLogController.AddHandOpenLog | 0x4500B0 | Separate public reveal queue |
| EffectTaskHandOpen constructor / WaitFrameStep | 0x5755B0 / 0x575070 | Positive show modes; avoid raw full-hand arrays |
| ActivateConfirmToggle.OnClickToggle | 0x1044E80 | Player-requested native toggle cycle |
| ActivateConfirmToggle.SetIcon | 0x1045430 | Visible mode announcements |
| ActivateConfirmToggle.SetManualType | 0x10454A0 | Native setting and interaction constraints |

These RVAs are build-specific research evidence, not hardcoded memory offsets
used by the mod. Harmony uses the current generated bindings.
The toggle's native enum is Default / Off / On; Default is spoken as Auto.
The old attack-selection callback runs only in a restricted client state. The
replacement reads the native attack record at creation. A canceled attack has
target position -1; a direct/life attack uses a non-monster target and icon
record. A failed lookup never becomes a direct attack. Target type 0 is an
effect target; type 1 shares ACTION_LOCKON but is a non-target selection.

The hand reader now uses AddCardMoveLog's exact CardStatus snapshots and native
IsCardKnown(status) gate before any identity lookup. It also captures the native
ACTION_SEARCH disclosure directly from BuffMoveData: that explicit public path
can name a card even when both statuses are unknown and no HandOpen follows.
Hidden moves retain only
unique ID and location. Positive HandOpen modes supply a separate, public
ACTION_HANDOPEN record; zero closes a reveal and negative annotation modes
are not spoken. No full-hand identity arrays are read. Correlation requires
the same unique ID, owner and location, and resets on new activations, phase/
turn changes, hand/deck shuffle/reset and duel boundaries. Hand shuffles break
identity correlation with earlier hidden draws. Public additions no
longer wait for PushBuff, which can merge or delay records.

Critical speech interrupts stale focus once; later targets and automatic
prompts queue behind it until deliberate player input. Activation speech uses
the same policy so a target cannot cut off the activating card's name.

## Confirmed checks

The supported native binary hash was rechecked for the October 21 repair.
The current build, offline event checks and deployment audit are recorded in
`docs/releases/v0.1.1-publication-audit.md`. The initial checks below describe
0.1.0 and do not establish live behavior of the new event hooks.

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
