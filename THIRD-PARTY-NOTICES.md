# Third-party notices

## BlindDuel

Original accessibility mod by RealAmethyst and contributors, including
vandercast. Baseline commit `340b0f1cd8df687314f759c59c9b94f46544119a`.
GNU LGPL v3. The original license is retained as `LICENSE`, with incorporated
GPL v3 text in `COPYING`. Millennium Eye's recovery and modifications are
dated 2026-10-13. See [provenance](docs/PROVENANCE.md).

## Tolk 1.0

Copyright Davy Kager and contributors. GNU LGPL v3.
[Source](https://github.com/dkager/tolk/tree/e5149f0cb6ef9b941673017e0e7b7c409e485fbe).
Built from unchanged source for Windows x64 using the C API, with the static
MSVC runtime and JNI disabled. The source/build instructions and upstream
copyright notices are included in the third-party source release asset.
License: `third-party/licenses/Tolk-LGPL-3.0.txt`.

## NVDA Controller Client API 2.0, NVDA 2026.2

Copyright NV Access and contributors. GNU LGPL v2.1.
Official x64 client from
[NV Access's versioned download](https://download.nvaccess.org/releases/2026.2/nvda_2026.2_controllerClient.zip),
renamed from `x64/nvdaControllerClient.dll` to `nvdaControllerClient64.dll` for
Tolk's loader. The DLL is unmodified. Its official source archive at tag object
`96ad7befb237e285a41649fd30f0b01be05ba200` is included in the third-party source
asset, together with its licenses and build scripts. License:
`third-party/licenses/NVDA-LGPL-2.1.txt`.

## SDL 3.4.0

Copyright the SDL contributors. Zlib license.
[Official Windows x64 release](https://github.com/libsdl-org/SDL/releases/tag/release-3.4.0).
The DLL is unmodified and matches the predecessor's installed SDL3 bytes.
License: `third-party/licenses/SDL-Zlib.txt`.

## MelonLoader 0.7.2 and .NET 6 Desktop Runtime

MelonLoader by LavaGang and contributors, Apache License 2.0; see
`third-party/licenses/MelonLoader-Apache-2.0.txt` and the
[upstream release](https://github.com/LavaGang/MelonLoader/releases/tag/v0.7.2).
Its package includes components with their own notices and licenses. The
manager downloads the pinned official loader package separately.

The manager also declares the official Microsoft .NET 6.0.36 x64 Desktop
Runtime prerequisite. It is downloaded separately from Microsoft's versioned
distribution. Installed game binaries and generated IL2CPP game references
are not distributed by Millennium Eye.

These libraries are dynamically loaded. Users may rebuild/replace LGPL
components; standalone installer integrity checks apply to the published
package before installation, not to modified private builds or runtime loading.
