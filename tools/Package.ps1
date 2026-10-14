[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$GamePath,
    [Parameter(Mandatory=$true)][string]$NativeDirectory,
    [Parameter(Mandatory=$true)][string]$OutputDirectory
)
$ErrorActionPreference = 'Stop'
$repo = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$output = [IO.Path]::GetFullPath($OutputDirectory)
if (Test-Path -LiteralPath $output) { throw 'Use a new, empty output directory; existing package artifacts are preserved.' }
$mod = Join-Path $repo 'src\MillenniumEye\bin\Release\net6.0\MillenniumEye.dll'
if (-not (Test-Path -LiteralPath $mod)) { throw 'Build the release DLL first.' }
$gameHash = (Get-FileHash -LiteralPath (Join-Path $GamePath 'GameAssembly.dll') -Algorithm SHA256).Hash
if ($gameHash -ne 'CA24A284A86DC394F6B0CBEAAD455D29E5F6B95FC8DB8FC42812B29DE3C7620C') { throw 'Unsupported game build for 0.1.0 packaging.' }
$payload = Join-Path $output 'standalone\files'
$support = Join-Path $payload 'Accessibility\MillenniumEye'
New-Item -ItemType Directory -Path (Join-Path $payload 'Mods'),$support | Out-Null
Copy-Item -LiteralPath $mod -Destination (Join-Path $payload 'Mods\BlindDuel.dll')
foreach ($name in @('Tolk.dll','nvdaControllerClient64.dll','SDL3.dll')) {
    Copy-Item -LiteralPath (Join-Path $NativeDirectory $name) -Destination (Join-Path $payload $name)
}
Copy-Item -LiteralPath (Join-Path $repo 'README.md'),(Join-Path $repo 'CHANGELOG.md'),(Join-Path $repo 'LICENSE'),(Join-Path $repo 'COPYING'),(Join-Path $repo 'THIRD-PARTY-NOTICES.md') -Destination $support
New-Item -ItemType Directory -Path (Join-Path $support 'docs'),(Join-Path $support 'third-party') | Out-Null
foreach ($doc in @('BUILDING.md','CONTROLS.md','PROVENANCE.md','VALIDATION.md')) {
    Copy-Item -LiteralPath (Join-Path $repo ('docs\' + $doc)) -Destination (Join-Path $support 'docs')
}
Copy-Item -LiteralPath (Join-Path $repo 'third-party\licenses') -Destination (Join-Path $support 'third-party') -Recurse
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'install\Setup.ps1') -Destination $support
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'install\Install-MillenniumEye.ps1'),(Join-Path $PSScriptRoot 'install\Restore-MillenniumEye.ps1'),(Join-Path $repo 'README.md') -Destination (Join-Path $output 'standalone')
$prefix = $payload.TrimEnd([char[]]'\/') + [IO.Path]::DirectorySeparatorChar
$files = @(Get-ChildItem -LiteralPath $payload -File -Recurse | Sort-Object FullName | ForEach-Object {
    [pscustomobject]@{path=$_.FullName.Substring($prefix.Length).Replace('\','/');sha256=(Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash}
})
$metadata = [ordered]@{
    version='0.1.0'
    gameSha256=$gameHash
    loaderSha256='8F329F102F2A8323816206C858C8117AEB8EE9B8502056997E849B61B4D8E0B7'
    legacyModHashes=@('A68D718166F6A0CA8EE639B456A50EDA2DD0C90C4D680831F91107A5E9487F52','0FF15B0C7DC2F2EFA2C763754522975EA3E64AF5A41C0CFB03B976052356AF7A')
    allowedExistingLibraries=@{
        'Tolk.dll'=@('C4FB11D3ED236F27532C7AB8370EBDE75133F322A069450F17E38D7548197225')
        'nvdaControllerClient64.dll'=@('796B9342EFF8AB3F99A9C551CA0D615AB7DE4944F99A69CE055F93A3ABEAF377')
        'SDL3.dll'=@('2632E21625861A0DC106F2B1ABB649E610D8BE88534BA74C76A763ABE5AEFAA3')
    }
    files=$files
}
$metadata | ConvertTo-Json -Depth 20 | Set-Content -LiteralPath (Join-Path $support 'release.json') -Encoding UTF8
. (Join-Path $PSScriptRoot 'install\Setup.ps1')
$null = Get-MillenniumEyePackage -PackageRoot $payload
Compress-Archive -Path (Join-Path $output 'standalone\*') -DestinationPath (Join-Path $output 'MillenniumEye-v0.1.0-beta-win-x64.zip') -CompressionLevel Optimal
Write-Output "Standalone package staged and checksummed at $output"
