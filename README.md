# Millennium Eye

Millennium Eye is a screen reader accessibility mod for the Windows Steam
version of **Yu-Gi-Oh! Master Duel**. It continues **BlindDuel**, originally
created by **RealAmethyst**, with contributions from the BlindDuel community.
It reads native menus, selected cards and public duel information through Tolk.

**[Download 0.1.2 beta](https://github.com/buu420/MillenniumEye/releases/tag/v0.1.2)**
or install through Amethyst's Accessibility Mod Manager: add the **buu420**
source under **Sources**, refresh it, and choose **Yu-Gi-Oh! Master Duel —
Millennium Eye (Beta)**. The catalog is [buu-s-mods](https://github.com/buu420/buu-s-mods).

Version **0.1.0** is the first release under this name. It is a community
continuation, with a fresh version sequence. The inherited menus and repaired
duel features need broader player testing; this release does not claim a
verified complete playthrough.

## Duel repairs

- Read the opponent's hand, main-deck and extra-deck counts, including zero.
- Read both current ATK and DEF for visible monsters in either battle position.
  Link monsters retain their native absence of DEF; active Trap Monsters use
  their current monster values.
- Refresh card details when selection, card information or a public effect
  changes. Empty or hidden selections clear previous readable details.
- Announce opponent attacks, the attacking monster and the attacked card's
  field location, including direct attacks.
- Read effect targets with their field zone or pile location, including the
  graveyard. Non-target choices are spoken as "Selected".
- Name public hand additions from the native movement log, including multi-card
  searches. Later hand reveals name the shown card without identifying an
  earlier hidden move. Hidden draws and unrevealed searches stay unidentified.
- Queue automatic response prompts behind critical duel information; deliberate
  navigation can interrupt the speech.
- Read and cycle the game's visible activation-confirmation toggle using its
  existing action. The player chooses every effect and response.
- Repair the duel confirmation and home submenu hooks for the current game.

## Controls

| Key | Action |
| --- | --- |
| Shift+P | Read all three opponent pile counts |
| Shift+C | Focus opponent hand, announcing its count |
| Shift+D | Focus opponent extra deck, announcing its count |
| F8 | Cycle the native activation-confirmation toggle |
| Ctrl+F8 | Read the visible On / Auto / Off mode |
| Ctrl+Up / Ctrl+Down | Read previous / next card-detail line |
| Right stick Up / Down | Read previous / next card-detail line |

If the native toggle is disabled, choose **Switch** or **Switch+Hold** for
Activation Confirmation in the game's Duel settings. The mod announces that
requirement and preserves the setting. Draw/standby response timing still
needs live verification. Existing native controller toggle actions are also
announced. See [the inherited controls](docs/CONTROLS.md) for navigation.

## Installation

Initial supported target: **Windows x64, Master Duel 2.8.0, Steam build
24462996**, with **MelonLoader 0.7.2 x64** and **.NET 6 Desktop Runtime**.
The installer checks the supported game binary. Updated game builds need a
compatibility check before this package will install.

The manager package declares pinned, hash-verified automatic downloads for
MelonLoader and its runtime. It includes Tolk, the NVDA controller client and
SDL3. Its read-only pre-install script checks the game and conflicts; the
manager handles file replacements, backups and uninstall receipts.

If BlindDuel is registered as installed in the manager, uninstall that entry
before installing Millennium Eye from the Buu420 source.

For the standalone ZIP:

1. Close Master Duel and install the prerequisites above. The
   [MelonLoader 0.7.2 installer](https://github.com/LavaGang/MelonLoader/releases/tag/v0.7.2)
   supplies the loader and its runtime.
2. Extract `MillenniumEye-v0.1.2-beta-win-x64.zip` into a separate folder.
3. Open PowerShell in that folder and run:

   ```powershell
   powershell -NoProfile -ExecutionPolicy Bypass -File .\Install-MillenniumEye.ps1 -GamePath 'YOUR MASTER DUEL FOLDER'
   ```

4. Start your screen reader, then launch Master Duel through Steam. The first
   launch may take longer while MelonLoader generates current game bindings.

The package deliberately replaces **`Mods/BlindDuel.dll` in place**. Its
assembly is now named MillenniumEye and the loader displays Millennium Eye
0.1.2. Keep one copy of the reader; a second `Mods/MillenniumEye.dll` would
duplicate hooks. Recognized older BlindDuel builds are backed up. Unknown
readers or shared libraries are preserved and reported as conflicts.

The standalone installer prints its rollback directory. Close the game, then
run `Restore-MillenniumEye.ps1 -GamePath 'YOUR MASTER DUEL FOLDER' -BackupPath
'THE PRINTED BACKUP FOLDER'` from the extracted release. It restores original
files and refuses to overwrite files that changed after installation.
Saves, game settings and loader preferences are outside the replacement list.

## Validation and reports

The 0.1.2 repair has offline event regressions in addition to the original
21 reader checks and current binding checks. The 25 installation checks cover backup,
restoration, duplicate readers, unsupported builds, corrupt payloads and copy
failure rollback and resuming an interrupted restore. Live duel features and
controller input remain unconfirmed. The player confirmed audible
title-screen and card/menu speech with 0.1.0; that does not verify this repair. See
[validation and native research](docs/VALIDATION.md).

[Report a problem](https://github.com/buu420/MillenniumEye/issues) with your
game version, control used, screen or duel phase, expected information and
what was spoken. The log is `Mods/MillenniumEye_debug.log`; review it before
sharing because duel/menu logs may contain player-visible personal text.

## Development and credits

[Build instructions](docs/BUILDING.md), [source provenance](docs/PROVENANCE.md),
and [third-party notices](THIRD-PARTY-NOTICES.md) are included. The source
repository excludes the game, generated game bindings, saves and player logs.

Original mod: RealAmethyst and the BlindDuel contributors, including
vandercast's zone-browsing contribution. Millennium Eye recovery, repairs,
packaging and maintenance: buu420. This project is an independent community
continuation and does not claim to be an official Konami or Amethyst release.

Licensed under **GNU LGPL v3**, retaining the original license. See [LICENSE](LICENSE)
and the incorporated [GPL v3 terms](COPYING). Dependency licenses are separate.
