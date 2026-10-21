[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$GamePath)
$ErrorActionPreference = 'Stop'
$payloadRoot = Join-Path $PSScriptRoot 'files'
. (Join-Path $payloadRoot 'Accessibility\MillenniumEye\Setup.ps1')
$backup = Install-MillenniumEye -GameRoot $GamePath -PackageRoot $payloadRoot
Write-Output "Millennium Eye 0.1.1 installed. Rollback backup: $backup"
