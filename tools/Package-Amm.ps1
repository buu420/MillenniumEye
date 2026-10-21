[CmdletBinding()]
param(
    [Parameter(Mandatory=$true)][string]$PayloadRoot,
    [Parameter(Mandatory=$true)][string]$CatalogRoot,
    [Parameter(Mandatory=$true)][string]$AuthorCli,
    [Parameter(Mandatory=$true)][string]$OutputZip,
    [Parameter(Mandatory=$true)][string]$StageDirectory
)
$ErrorActionPreference = 'Stop'
if (Test-Path -LiteralPath $StageDirectory) { throw 'Use a new stage directory.' }
New-Item -ItemType Directory -Path $StageDirectory | Out-Null
Get-ChildItem -LiteralPath $PayloadRoot -Force | Copy-Item -Destination $StageDirectory -Recurse
Copy-Item -LiteralPath (Join-Path $PSScriptRoot 'install\Manager-PreInstall.ps1') -Destination $StageDirectory
& $AuthorCli package build --source $StageDirectory --game master-duel --version 0.1.1 --target-platform windows --output $OutputZip --project $CatalogRoot --json
if ($LASTEXITCODE -ne 0) { throw 'AMM package build failed.' }
# The check runs only from verified staging. Do not install its root launcher.
Add-Type -AssemblyName System.IO.Compression.FileSystem
$zip = [IO.Compression.ZipFile]::Open($OutputZip, [IO.Compression.ZipArchiveMode]::Update)
try {
    $entry = $zip.GetEntry('manifest.json')
    $reader = [IO.StreamReader]::new($entry.Open())
    try { $manifest = $reader.ReadToEnd() | ConvertFrom-Json } finally { $reader.Dispose() }
    if ($manifest.gameId -ne 'master-duel' -or $manifest.pluginId -ne 'buu420' -or $manifest.modVersion -ne '0.1.1' -or
        $manifest.preInstall.executable -ne 'files/Manager-PreInstall.ps1' -or $manifest.preInstall.installToGameFolder -ne $false) {
        throw 'Unexpected AMM identity or pre-install contract.'
    }
    $actions = @($manifest.installActions | Where-Object { -not ($_.type -eq 'copyFile' -and $_.source -eq 'Manager-PreInstall.ps1' -and $_.target -eq 'Manager-PreInstall.ps1') })
    $manifest.installActions = $actions
    $bytes = [Text.UTF8Encoding]::new($false).GetBytes(($manifest | ConvertTo-Json -Depth 100))
    $stream = $entry.Open()
    try { $stream.SetLength(0); $stream.Write($bytes,0,$bytes.Length) } finally { $stream.Dispose() }
} finally { $zip.Dispose() }
& $AuthorCli package validate --zip $OutputZip --plugin buu420 --game master-duel --version 0.1.1 --json
if ($LASTEXITCODE -ne 0) { throw 'AMM package validation failed.' }
