[CmdletBinding()]
param([string]$gameFolder, [string]$modFolder)
$ErrorActionPreference = 'Stop'
try {
    if ([string]::IsNullOrWhiteSpace($gameFolder)) { throw 'The manager did not supply the game folder.' }
    . (Join-Path $PSScriptRoot 'Accessibility\MillenniumEye\Setup.ps1')
    $null = Assert-MillenniumEyeInstall -GameRoot $gameFolder -PackageRoot $PSScriptRoot -AllowMissingLoader
    Write-Output 'Millennium Eye package, supported game and conflict checks passed. The manager can install using its file receipts.'
    exit 0
} catch {
    Write-Error $_ -ErrorAction Continue
    exit 1
}
