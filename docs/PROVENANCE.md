# Source provenance

The repair began from the installed **BlindDuel.dll**, whose SHA-256 was
`A68D718166F6A0CA8EE639B456A50EDA2DD0C90C4D680831F91107A5E9487F52`.
Its assembly version was 1.0.0 and informational version identified commit
`340b0f1cd8df687314f759c59c9b94f46544119a`.

ILSpy 11.0.0.9375 recovered the managed mod source. The mutable recovery was
repaired against fresh Cpp2IL/Il2CppInterop bindings generated from the
installed game. Those generated game assemblies are references, not recovered
game implementations, and are not published here.

The original RealAmethyst/BlindDuel repository returned HTTP 404 during the
repair. For publication, a surviving
[fork retains the exact baseline commit](https://github.com/jpavonabian/BlindDuel/tree/340b0f1cd8df687314f759c59c9b94f46544119a),
including its [LGPL v3 license](https://github.com/jpavonabian/BlindDuel/blob/340b0f1cd8df687314f759c59c9b94f46544119a/LICENSE).
This repository retains that license verbatim. The old local source archive
contained fewer source files and was not used to replace the installed baseline.

Millennium Eye keeps the repaired managed recovery as its working source.
It includes decompiler-generated structure and does not claim to preserve
original formatting, comments or the full original Git history. File notices
identify recovery/changes by buu420 on October 13, 2026. The intermediate
local BlindDuel 1.0.1 repair was not a public release; Millennium Eye 0.1.0
contains that work under the new identity.

Ghidra was used to inspect selected current native game functions and verify
counts, live stats, visibility, public search logs and toggle semantics. Its
output was native pseudocode, not original source. Private game images,
metadata exports, logs and saves remain outside this public repository.
