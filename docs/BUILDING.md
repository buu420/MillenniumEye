# Building Millennium Eye

Install the .NET 8 SDK or newer. The mod targets .NET 6 to match MelonLoader
0.7.2. Install that loader into a legally installed Windows Steam copy of
Master Duel, then launch once so MelonLoader generates current IL2CPP bindings.

```powershell
dotnet build src/MillenniumEye/MillenniumEye.csproj -c Release '-p:GamePath=YOUR MASTER DUEL FOLDER'
dotnet run --project tests/ReaderTests/ReaderTests.csproj -c Release
dotnet run --project tests/DuelEventTests/DuelEventTests.csproj -c Release
powershell -NoProfile -File tests/Install.Tests.ps1
```

Output: `src/MillenniumEye/bin/Release/net6.0/MillenniumEye.dll`.
The release installs that assembly as `Mods/BlindDuel.dll` for in-place upgrades.
`InteropPath` can override the default `GamePath/MelonLoader/Il2CppAssemblies`.
Regenerate bindings after a game update; references generated for an older
GameAssembly are not a compatibility guarantee.

Check Harmony targets with the included metadata tool:

```powershell
dotnet run --project tools/Compatibility/Compatibility.csproj '-p:GamePath=YOUR MASTER DUEL FOLDER' -- 'PATH TO MillenniumEye.dll' 'GAME/MelonLoader/Il2CppAssemblies' 'GAME/MelonLoader/net6' 'compatibility.json'
```

The offline reader project links production formatting, visibility-policy,
detail-buffer and toggle code to minimal native boundary doubles. CI runs
those checks, production duel-event hooks against native boundary doubles,
and temporary-folder installation checks. CI does not compile
against proprietary game bindings or run Master Duel.

`tools/Package.ps1` stages public payload files from the built DLL and verified
native dependencies. It needs the game path, dependency directory and output
directory. `tools/Package-Amm.ps1` wraps the payload using AMM Author CLI
0.30.0. The resulting manager manifest and ZIP must validate before publication.

Tolk 1.0 is built from unchanged source at
`e5149f0cb6ef9b941673017e0e7b7c409e485fbe`, with MSVC 2022 x64 and a static
runtime. In a Visual Studio x64 developer prompt, from its `src` directory:

```text
nmake /nologo CFLAGS="/nologo /O2 /EHsc /LD /MT /Gw /W4 /WL /D_EXPORTING /DUNICODE"
```

JNI support is not enabled; this mod uses the C API. The complete Tolk source
and official NVDA source archive are supplied as the release's
`MillenniumEye-v0.1.2-third-party-source.zip` asset. See the notices for versions.
