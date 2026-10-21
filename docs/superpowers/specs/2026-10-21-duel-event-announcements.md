# Duel event announcement repair

The player confirmed this scope on October 21, 2026: automatically read the
opponent's attacking monster and attack target; read effect targets on the
field and in piles including the graveyard; name cards publicly revealed
when added to the opponent's hand. Deliver the visible information during
the native event, before the player chooses a response.

Use current generated bindings and native visibility decisions. Never infer
a direct attack from a failed card lookup, disclose an unrevealed identity,
attribute an effect to a stale activation, or automate a response. A target
announcement may say "Target" without guessing which effect caused it.
Include card location so identical cards in different field zones can be
distinguished. Preserve every target in multi-card events.

An addition's origin requires a public identity in that movement's native log
record. A later reveal names the currently revealed hand card only. Never
attribute an earlier hidden draw or move through its private unique ID, or
identify a hidden move using a previously cached reveal.

Automatic focus speech must queue behind critical event information. Explicit
player navigation may interrupt it. Reset pending speech policy and reveal
correlation at duel boundaries. Deployment must preserve saves and settings.
Offline checks do not establish live duel timing or audible speech.
