# Millennium Eye first release

The user authorized a new repository named Millennium Eye, local deployment,
and publication through Amethyst's Accessibility Mod Manager. Release 0.1.0 is
the first release under this name, continuing RealAmethyst's BlindDuel; it is
a beta because the repaired duel behavior has not been player-tested.

Preserve the confirmed repair scope: opponent hand/deck/extra-deck counts;
both live ATK and DEF; fresh selected and activated card details; names only
for publicly revealed opponent searches; accessible native activation toggles.
Retain the original authors and LGPL v3. Publish no game binaries, generated
game bindings, user saves, personal logs, account identifiers, or credentials.

Use the existing buu420 author catalog without changing its other games or
releases. Supply the required speech/controller libraries and a pinned loader
dependency. Replace the legacy Mods/BlindDuel.dll in place with the new
MillenniumEye assembly to avoid loading both readers. Protect settings and
saves, retain rollback backups, and distinguish offline/startup evidence from
live duel, controller and audible speech evidence.
