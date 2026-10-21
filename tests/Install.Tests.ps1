$ErrorActionPreference = 'Stop'
. (Join-Path $PSScriptRoot '..\tools\install\Setup.ps1')
# These temporary folders are not a running game. Exercise the production
# process guard against a controlled host inventory, without closing a player.
$script:fixtureProcessRunning = $false
function Get-Process {
    param([string]$Name, [object]$ErrorAction)
    if ($script:fixtureProcessRunning) { [pscustomobject]@{ProcessName='masterduel'} }
}
$taskRoot = Join-Path ([IO.Path]::GetTempPath()) ('MillenniumEye-tests-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $taskRoot | Out-Null
$script:checks = 0
function Assert-Check([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw $Message }
    $script:checks++
}
function Expect-Refusal([scriptblock]$Action, [string]$Expected) {
    try { & $Action } catch {
        Assert-Check ($_.Exception.Message -like "*$Expected*") "Wrong refusal: $_"
        return
    }
    throw "Expected refusal: $Expected"
}
function New-Fixture([string]$Name) {
    $game = Join-Path $taskRoot ($Name + '-game')
    $package = Join-Path $taskRoot ($Name + '-package')
    New-Item -ItemType Directory -Path $game,(Join-Path $package 'Mods'),(Join-Path $package 'Accessibility\MillenniumEye') | Out-Null
    [IO.File]::WriteAllText((Join-Path $game 'masterduel.exe'), 'fixture')
    [IO.File]::WriteAllText((Join-Path $game 'GameAssembly.dll'), 'game fixture')
    [IO.File]::WriteAllText((Join-Path $game 'version.dll'), 'loader fixture')
    [IO.File]::WriteAllText((Join-Path $package 'Mods\BlindDuel.dll'), 'new mod fixture')
    [IO.File]::WriteAllText((Join-Path $package 'Tolk.dll'), 'new speech fixture')
    $meta = @{
        version='0.1.1'; gameSha256=(Get-FileHash (Join-Path $game 'GameAssembly.dll')).Hash
        loaderSha256=(Get-FileHash (Join-Path $game 'version.dll')).Hash
        legacyModHashes=@(); allowedExistingLibraries=@{ 'Tolk.dll'=@() }
        files=@(
            @{path='Mods/BlindDuel.dll';sha256=(Get-FileHash (Join-Path $package 'Mods\BlindDuel.dll')).Hash},
            @{path='Tolk.dll';sha256=(Get-FileHash (Join-Path $package 'Tolk.dll')).Hash}
        )
    }
    $meta | ConvertTo-Json -Depth 12 | Set-Content (Join-Path $package 'Accessibility\MillenniumEye\release.json') -Encoding UTF8
    return @{game=$game;package=$package;meta=$meta}
}
$fresh = New-Fixture 'fresh'
$script:fixtureProcessRunning = $true
Expect-Refusal { Install-MillenniumEye -GameRoot $fresh.game -PackageRoot $fresh.package } 'Close Master Duel'
$script:fixtureProcessRunning = $false
$backup = Install-MillenniumEye -GameRoot $fresh.game -PackageRoot $fresh.package
Assert-Check (Test-Path (Join-Path $fresh.game 'Mods\BlindDuel.dll')) 'Fresh install did not create the reader.'
Restore-MillenniumEye -GameRoot $fresh.game -BackupRoot $backup
Assert-Check (-not (Test-Path (Join-Path $fresh.game 'Mods\BlindDuel.dll'))) 'Fresh uninstall left the reader active.'

$legacy = New-Fixture 'legacy'
New-Item -ItemType Directory -Path (Join-Path $legacy.game 'Mods'),(Join-Path $legacy.game 'LocalSave') | Out-Null
[IO.File]::WriteAllText((Join-Path $legacy.game 'Mods\BlindDuel.dll'), 'old mod fixture')
[IO.File]::WriteAllText((Join-Path $legacy.game 'LocalSave\save'), 'save fixture')
$legacy.meta.legacyModHashes=@((Get-FileHash (Join-Path $legacy.game 'Mods\BlindDuel.dll')).Hash)
$legacy.meta | ConvertTo-Json -Depth 12 | Set-Content (Join-Path $legacy.package 'Accessibility\MillenniumEye\release.json') -Encoding UTF8
$backup = Install-MillenniumEye -GameRoot $legacy.game -PackageRoot $legacy.package
$again = Install-MillenniumEye -GameRoot $legacy.game -PackageRoot $legacy.package
Assert-Check ([IO.File]::ReadAllText((Join-Path $legacy.game 'LocalSave\save')) -eq 'save fixture') 'Install changed a save.'
Restore-MillenniumEye -GameRoot $legacy.game -BackupRoot $again
Restore-MillenniumEye -GameRoot $legacy.game -BackupRoot $backup
Assert-Check ([IO.File]::ReadAllText((Join-Path $legacy.game 'Mods\BlindDuel.dll')) -eq 'old mod fixture') 'Legacy restore did not restore its original bytes.'

$invalid = New-Fixture 'invalid'
[IO.File]::WriteAllText((Join-Path $invalid.package 'Tolk.dll'), 'corrupted fixture')
Expect-Refusal { Install-MillenniumEye -GameRoot $invalid.game -PackageRoot $invalid.package } 'checksum'
Assert-Check (-not (Test-Path (Join-Path $invalid.game 'Mods\BlindDuel.dll'))) 'Corrupt package wrote files before refusing.'

$duplicate = New-Fixture 'duplicate'
New-Item -ItemType Directory -Path (Join-Path $duplicate.game 'Mods') | Out-Null
[IO.File]::WriteAllText((Join-Path $duplicate.game 'Mods\MillenniumEye.dll'), 'duplicate fixture')
Expect-Refusal { Install-MillenniumEye -GameRoot $duplicate.game -PackageRoot $duplicate.package } 'both readers'

$renamed = New-Fixture 'renamed-duplicate'
New-Item -ItemType Directory -Path (Join-Path $renamed.game 'Mods') | Out-Null
[IO.File]::WriteAllText((Join-Path $renamed.game 'Mods\BlindDuel-backup.dll'), 'old renamed reader')
$renamed.meta.legacyModHashes=@((Get-FileHash (Join-Path $renamed.game 'Mods\BlindDuel-backup.dll')).Hash)
$renamed.meta | ConvertTo-Json -Depth 12 | Set-Content (Join-Path $renamed.package 'Accessibility\MillenniumEye\release.json') -Encoding UTF8
Expect-Refusal { Install-MillenniumEye -GameRoot $renamed.game -PackageRoot $renamed.package } 'both readers'

$unsupported = New-Fixture 'unsupported'
[IO.File]::WriteAllText((Join-Path $unsupported.game 'GameAssembly.dll'), 'updated game fixture')
Expect-Refusal { Install-MillenniumEye -GameRoot $unsupported.game -PackageRoot $unsupported.package } 'unsupported game'
[IO.File]::WriteAllText((Join-Path $unsupported.game 'GameAssembly.dll'), 'game fixture')
[IO.File]::WriteAllText((Join-Path $unsupported.game 'version.dll'), 'foreign proxy fixture')
Expect-Refusal { Install-MillenniumEye -GameRoot $unsupported.game -PackageRoot $unsupported.package } 'loader'

$unknown = New-Fixture 'unknown'
New-Item -ItemType Directory -Path (Join-Path $unknown.game 'Mods') | Out-Null
[IO.File]::WriteAllText((Join-Path $unknown.game 'Mods\BlindDuel.dll'), 'unknown mod fixture')
Expect-Refusal { Install-MillenniumEye -GameRoot $unknown.game -PackageRoot $unknown.package } 'unrecognized reader'
Assert-Check ([IO.File]::ReadAllText((Join-Path $unknown.game 'Mods\BlindDuel.dll')) -eq 'unknown mod fixture') 'Unknown reader was overwritten.'

$paths = New-Fixture 'paths'
$paths.meta.files[0].path='../outside.dll'
$paths.meta | ConvertTo-Json -Depth 12 | Set-Content (Join-Path $paths.package 'Accessibility\MillenniumEye\release.json') -Encoding UTF8
Expect-Refusal { Install-MillenniumEye -GameRoot $paths.game -PackageRoot $paths.package } 'unsafe path'

$failure = New-Fixture 'failure'
New-Item -ItemType Directory -Path (Join-Path $failure.game 'Tolk.dll') | Out-Null
Expect-Refusal { Install-MillenniumEye -GameRoot $failure.game -PackageRoot $failure.package } 'file'
Assert-Check (-not (Test-Path (Join-Path $failure.game 'Mods\BlindDuel.dll'))) 'Failed install left a new reader active.'

$midCopy = New-Fixture 'mid-copy'
$taskCopyImplementation = (Get-Item Function:Copy-MillenniumEyeFile).ScriptBlock
function Copy-MillenniumEyeFile {
    param([string]$Source, [string]$Destination)
    if ($Destination -eq (Join-Path $midCopy.game 'Tolk.dll')) { throw 'Controlled copy failure.' }
    Copy-Item -LiteralPath $Source -Destination $Destination -Force -ErrorAction Stop
}
Expect-Refusal { Install-MillenniumEye -GameRoot $midCopy.game -PackageRoot $midCopy.package } 'Controlled copy failure'
Set-Item Function:Copy-MillenniumEyeFile -Value $taskCopyImplementation
Assert-Check (-not (Test-Path (Join-Path $midCopy.game 'Mods\BlindDuel.dll'))) 'Copy rollback left the first copied reader active.'

$missingLoader = New-Fixture 'missing-loader'
Remove-Item -LiteralPath (Join-Path $missingLoader.game 'version.dll')
Expect-Refusal { Install-MillenniumEye -GameRoot $missingLoader.game -PackageRoot $missingLoader.package } 'MelonLoader'
$null = Assert-MillenniumEyeInstall -GameRoot $missingLoader.game -PackageRoot $missingLoader.package -AllowMissingLoader
Assert-Check (-not (Test-Path (Join-Path $missingLoader.game 'Mods\BlindDuel.dll'))) 'AMM pre-install check changed game files.'

$restore = New-Fixture 'restore'
$backup = Install-MillenniumEye -GameRoot $restore.game -PackageRoot $restore.package
[IO.File]::WriteAllText((Join-Path $restore.game 'Mods\BlindDuel.dll'), 'user changed mod fixture')
Expect-Refusal { Restore-MillenniumEye -GameRoot $restore.game -BackupRoot $backup } 'changed'
Assert-Check ([IO.File]::ReadAllText((Join-Path $restore.game 'Mods\BlindDuel.dll')) -eq 'user changed mod fixture') 'Restore overwrote a changed reader.'

$resume = New-Fixture 'resume-restore'
New-Item -ItemType Directory -Path (Join-Path $resume.game 'Mods') | Out-Null
[IO.File]::WriteAllText((Join-Path $resume.game 'Mods\BlindDuel.dll'), 'old resume reader')
[IO.File]::WriteAllText((Join-Path $resume.game 'Tolk.dll'), 'old resume speech')
$resume.meta.legacyModHashes=@((Get-FileHash (Join-Path $resume.game 'Mods\BlindDuel.dll')).Hash)
$resume.meta.allowedExistingLibraries.'Tolk.dll'=@((Get-FileHash (Join-Path $resume.game 'Tolk.dll')).Hash)
$resume.meta | ConvertTo-Json -Depth 12 | Set-Content (Join-Path $resume.package 'Accessibility\MillenniumEye\release.json') -Encoding UTF8
$resumeBackup = Install-MillenniumEye -GameRoot $resume.game -PackageRoot $resume.package
function Copy-MillenniumEyeFile {
    param([string]$Source, [string]$Destination)
    if ($Destination -eq (Join-Path $resume.game 'Tolk.dll')) { throw 'Controlled restore failure.' }
    Copy-Item -LiteralPath $Source -Destination $Destination -Force -ErrorAction Stop
}
Expect-Refusal { Restore-MillenniumEye -GameRoot $resume.game -BackupRoot $resumeBackup } 'Controlled restore failure'
Set-Item Function:Copy-MillenniumEyeFile -Value $taskCopyImplementation
Restore-MillenniumEye -GameRoot $resume.game -BackupRoot $resumeBackup
Assert-Check ([IO.File]::ReadAllText((Join-Path $resume.game 'Mods\BlindDuel.dll')) -eq 'old resume reader') 'Retry did not retain the restored reader.'
Assert-Check ([IO.File]::ReadAllText((Join-Path $resume.game 'Tolk.dll')) -eq 'old resume speech') 'Retry did not finish restoring speech.'
Restore-MillenniumEye -GameRoot $resume.game -BackupRoot $resumeBackup

Write-Output "$script:checks installation checks passed. Fixtures retained at $taskRoot"
