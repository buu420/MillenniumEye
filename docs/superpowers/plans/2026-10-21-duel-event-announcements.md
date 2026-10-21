# Duel event announcements implementation plan

> **For agentic workers:** Use superpowers:executing-plans to implement this plan task by task.

**Goal:** Read public attacks, effect targets and revealed hand additions in time to inform a response.

**Architecture:** Capture new records from the native duel log at their creation,
retaining the event parameters needed to distinguish target types. Format immutable
public snapshots, and protect event speech from automatic focus interruptions until
the player navigates. Deduplicate public records without identifying hidden movements.

**Tech Stack:** C#/.NET 6, Harmony, current Il2CppInterop bindings, Tolk; offline .NET 8 harness.

**Spec:** `docs/superpowers/specs/2026-10-21-duel-event-announcements.md`

## Global constraints

- Master Duel 2.8.0 build 24462996; GameAssembly hash documented in VALIDATION.md.
- Native hidden-information boundary; no automatic Confirm or duel-state writes.
- Install as Mods/BlindDuel.dll and retain LGPL attribution and dependencies.
- Report offline, loader, speech and live gameplay verification separately.

## Review focus

- Repeated identical cards and multiple targets must all be announced.
- Failed or hidden target identity must never become a direct attack.
- Non-target lock-on selections must not be mislabeled as effect targets.
- Public reveal and move ordering must not turn a draw into an identified search.
- Automatic prompts must preserve speech; intentional navigation must remain usable.

### Task 1: Public native event capture and speech delivery

**Files:** Replace AttackTargetHandler.cs; add DuelEventReader.cs, PatchDuelEventLogs.cs,
DuelEventDelivery.cs; modify ScreenReader.cs, Speech.cs, BlindDuelCore.cs,
SDLController.cs, PatchDuelClientAwake.cs and DuelState.cs. Add an offline DuelEventTests project.

- [x] Add failing attack, direct/canceled attack, hidden identity, target location,
  non-target lock-on, repeated/multiple target and automatic-prompt ordering tests.
- [x] Run the harness and confirm the failures; implement native log snapshot hooks
  and event formatting without later engine card lookups.
- [x] Run the real handlers with boundary doubles, build against current bindings,
  and check all Harmony targets.

### Task 2: Public hand addition coverage

**Files:** PatchRevealedDeckSearch.cs and focused reveal state/helper as required by
the native AddHandOpenLog/AddCardMoveLog/PushBuff evidence; DuelEventTests.

- [x] Add failing tests for ordinary search, shown-before-move and shown-after-move,
  multiple revealed cards, duplicate events, hidden draws and visibility reset.
- [x] Implement the smallest native public-data adapter/correlation justified by
  the research. Record native field semantics in VALIDATION.md.
- [x] Confirm the new regressions and existing 21 reader checks pass.

### Task 3: Review, deploy and release

**Files:** Version metadata, packaging scripts, release notes, validation audit.

- [x] Review the whole repair, fix material findings and rerun affected checks.
- [x] Run install regressions, package and validate beta 0.1.2 with upgrade support
  for the installed 0.1.0/0.1.1 readers. Back up and deploy only owned mod files.
- [x] Verify the deployed hashes and, where available, loader startup. Publish the
  source/package and manager update under the existing publication authorization.
  State any unperformed live duel/controller/speech tests explicitly.
