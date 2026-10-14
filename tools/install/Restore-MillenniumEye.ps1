[CmdletBinding()]
param([Parameter(Mandatory=$true)][string]$GamePath, [Parameter(Mandatory=$true)][string]$BackupPath)
$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot 'files\Accessibility\MillenniumEye\Setup.ps1')
Restore-MillenniumEye -GameRoot $GamePath -BackupRoot $BackupPath
Write-Output 'The backed-up reader and shared libraries were restored.'
