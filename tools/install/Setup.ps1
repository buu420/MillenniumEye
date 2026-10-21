# Millennium Eye installation helpers, buu420, 2026-10-13; LGPL-3.0.
Set-StrictMode -Version Latest

function Get-MillenniumEyePath {
    param([string]$Root, [string]$Relative)
    if ([string]::IsNullOrWhiteSpace($Relative) -or
        [IO.Path]::IsPathRooted($Relative) -or $Relative.Contains(':') -or
        @($Relative -split '[\\/]' | Where-Object { $_ -eq '..' -or $_ -eq '.' -or $_ -eq '' }).Count -gt 0) {
        throw "Unsafe path: $Relative"
    }
    $rootPath = [IO.Path]::GetFullPath($Root).TrimEnd([char[]]'\/')
    $fullPath = [IO.Path]::GetFullPath((Join-Path $rootPath $Relative))
    if (-not $fullPath.StartsWith($rootPath + [IO.Path]::DirectorySeparatorChar, [StringComparison]::OrdinalIgnoreCase)) {
        throw "Unsafe path: $Relative"
    }
    $probe = $fullPath
    while ($probe.Length -ge $rootPath.Length) {
        if (Test-Path -LiteralPath $probe) {
            $item = Get-Item -LiteralPath $probe -Force
            if (($item.Attributes -band [IO.FileAttributes]::ReparsePoint) -ne 0) { throw "Unsafe path through a reparse point: $Relative" }
        }
        if ($probe -eq $rootPath) { break }
        $probe = [IO.Path]::GetDirectoryName($probe)
    }
    return $fullPath
}

function Assert-MillenniumEyeClosed {
    if (@(Get-Process -Name masterduel -ErrorAction SilentlyContinue).Count -gt 0) {
        throw 'Close Master Duel before installing or restoring Millennium Eye.'
    }
}

function Get-MillenniumEyePackage {
    param([string]$PackageRoot)
    $metaPath = Get-MillenniumEyePath $PackageRoot 'Accessibility/MillenniumEye/release.json'
    $meta = Get-Content -LiteralPath $metaPath -Raw | ConvertFrom-Json
    if ($meta.version -ne '0.1.1' -or @($meta.files).Count -eq 0) { throw 'Unsupported Millennium Eye package metadata.' }
    $seen = @{}
    foreach ($entry in $meta.files) {
        $path = Get-MillenniumEyePath $PackageRoot $entry.path
        if ($entry.path -notmatch '^(Mods/BlindDuel\.dll|Tolk\.dll|nvdaControllerClient64\.dll|SDL3\.dll|Accessibility/MillenniumEye/.+)$') {
            throw "Unsafe path outside the owned payload: $($entry.path)"
        }
        if ($entry.path -eq 'Accessibility/MillenniumEye/release.json' -or $seen.ContainsKey($entry.path)) { throw 'Duplicate package file path.' }
        $seen[$entry.path] = $true
        if (-not (Test-Path -LiteralPath $path -PathType Leaf) -or
            (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash -ne $entry.sha256) {
            throw "Package checksum failed: $($entry.path)"
        }
    }
    if (-not $seen.ContainsKey('Mods/BlindDuel.dll')) { throw 'Package is missing its reader DLL.' }
    return $meta
}

function Assert-MillenniumEyeInstall {
    param([string]$GameRoot, [string]$PackageRoot, [switch]$AllowMissingLoader)
    Assert-MillenniumEyeClosed
    $meta = Get-MillenniumEyePackage $PackageRoot
    $exe = Get-MillenniumEyePath $GameRoot 'masterduel.exe'
    $assembly = Get-MillenniumEyePath $GameRoot 'GameAssembly.dll'
    if (-not (Test-Path -LiteralPath $exe -PathType Leaf) -or
        -not (Test-Path -LiteralPath $assembly -PathType Leaf) -or
        (Get-FileHash -LiteralPath $assembly -Algorithm SHA256).Hash -ne $meta.gameSha256) {
        throw 'Unsupported game build. Millennium Eye 0.1.1 supports Master Duel 2.8.0 / Steam build 24462996. No files were changed.'
    }
    $loader = Get-MillenniumEyePath $GameRoot 'version.dll'
    if (Test-Path -LiteralPath $loader) {
        if (-not (Test-Path -LiteralPath $loader -PathType Leaf) -or
            (Get-FileHash -LiteralPath $loader -Algorithm SHA256).Hash -ne $meta.loaderSha256) {
            throw 'Unrecognized loader/proxy. Install the supported MelonLoader 0.7.2 x64 without overwriting another loader.'
        }
    } elseif (-not $AllowMissingLoader) {
        throw 'Install MelonLoader 0.7.2 x64 and its .NET 6 Desktop Runtime before installing the standalone package.'
    }
    if (Test-Path -LiteralPath (Get-MillenniumEyePath $GameRoot 'Mods/MillenniumEye.dll')) {
        throw 'This package replaces Mods/BlindDuel.dll in place. Keeping both readers would duplicate hooks; move the other reader outside Mods first.'
    }
    $readerEntry = @($meta.files | Where-Object path -eq 'Mods/BlindDuel.dll')[0]
    $readerHashes = @($meta.legacyModHashes) + @($readerEntry.sha256)
    $modsPath = Get-MillenniumEyePath $GameRoot 'Mods'
    if (Test-Path -LiteralPath $modsPath -PathType Container) {
        foreach ($candidate in @(Get-ChildItem -LiteralPath $modsPath -File -Filter '*.dll' -Force)) {
            if ($candidate.Name -ieq 'BlindDuel.dll') { continue }
            $candidatePath = Get-MillenniumEyePath $GameRoot ('Mods/' + $candidate.Name)
            $candidateHash = (Get-FileHash -LiteralPath $candidatePath -Algorithm SHA256).Hash
            $assemblyName = $null
            try { $assemblyName = [Reflection.AssemblyName]::GetAssemblyName($candidatePath).Name } catch { }
            if ($candidateHash -in $readerHashes -or $assemblyName -in @('BlindDuel','MillenniumEye')) {
                throw "Keeping both readers would duplicate hooks: Mods/$($candidate.Name). Move that copy outside Mods first."
            }
        }
    }
    $metaRelative = 'Accessibility/MillenniumEye/release.json'
    $entries = @($meta.files) + @([pscustomobject]@{path=$metaRelative;sha256=(Get-FileHash -LiteralPath (Get-MillenniumEyePath $PackageRoot $metaRelative)).Hash})
    foreach ($entry in $entries) {
        $path = Get-MillenniumEyePath $GameRoot $entry.path
        if (-not (Test-Path -LiteralPath $path)) { continue }
        if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { throw "Expected a file at $($entry.path)." }
        $hash = (Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash
        if ($hash -eq $entry.sha256) { continue }
        if ($entry.path -eq 'Mods/BlindDuel.dll') {
            if ($hash -notin @($meta.legacyModHashes)) { throw 'Unrecognized reader DLL; it was preserved. Move it outside Mods before installing.' }
        } elseif ($entry.path -in @('Tolk.dll','nvdaControllerClient64.dll','SDL3.dll')) {
            $property = $meta.allowedExistingLibraries.PSObject.Properties[$entry.path]
            if ($null -eq $property -or $hash -notin @($property.Value)) { throw "Unrecognized shared library $($entry.path); the file was preserved." }
        }
    }
    return $meta
}

function Copy-MillenniumEyeFile {
    param([string]$Source, [string]$Destination)
    Copy-Item -LiteralPath $Source -Destination $Destination -Force -ErrorAction Stop
}

function Install-MillenniumEye {
    param([string]$GameRoot, [string]$PackageRoot)
    $meta = Assert-MillenniumEyeInstall -GameRoot $GameRoot -PackageRoot $PackageRoot
    $metaRelative = 'Accessibility/MillenniumEye/release.json'
    $entries = @($meta.files) + @([pscustomobject]@{path=$metaRelative;sha256=(Get-FileHash -LiteralPath (Get-MillenniumEyePath $PackageRoot $metaRelative)).Hash})
    $backupRelative = 'UserData/MillenniumEye/Backups/' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '-' + [Guid]::NewGuid().ToString('N')
    $backupRoot = Get-MillenniumEyePath $GameRoot $backupRelative
    New-Item -ItemType Directory -Path $backupRoot | Out-Null
    $receipt = @()
    $attempted = @()
    try {
        # Back up every existing file before the first replacement.
        foreach ($entry in $entries) {
            $target = Get-MillenniumEyePath $GameRoot $entry.path
            $present = Test-Path -LiteralPath $target -PathType Leaf
            $originalHash = $null
            if ($present) {
                $original = Get-MillenniumEyePath $backupRoot ('originals/' + $entry.path)
                New-Item -ItemType Directory -Path ([IO.Path]::GetDirectoryName($original)) -Force | Out-Null
                Copy-MillenniumEyeFile -Source $target -Destination $original
                $originalHash = (Get-FileHash -LiteralPath $original).Hash
            }
            $receipt += [pscustomobject]@{path=$entry.path;installedSha256=$entry.sha256;hadOriginal=$present;originalSha256=$originalHash}
        }
        # Retain recovery information even if a later copy or rollback fails.
        [pscustomobject]@{version=$meta.version;files=$receipt} | ConvertTo-Json -Depth 12 |
            Set-Content -LiteralPath (Join-Path $backupRoot 'receipt.json') -Encoding UTF8
        foreach ($entry in $entries) {
            $target = Get-MillenniumEyePath $GameRoot $entry.path
            New-Item -ItemType Directory -Path ([IO.Path]::GetDirectoryName($target)) -Force | Out-Null
            $attempted += $entry.path
            Copy-MillenniumEyeFile -Source (Get-MillenniumEyePath $PackageRoot $entry.path) -Destination $target
            if ((Get-FileHash -LiteralPath $target).Hash -ne $entry.sha256) { throw "Installed checksum failed: $($entry.path)" }
        }
    } catch {
        $installError = $_
        foreach ($entry in $receipt) {
            if ($entry.path -notin $attempted) { continue }
            $target = Get-MillenniumEyePath $GameRoot $entry.path
            if ($entry.hadOriginal) {
                Copy-MillenniumEyeFile -Source (Get-MillenniumEyePath $backupRoot ('originals/' + $entry.path)) -Destination $target
            } elseif (Test-Path -LiteralPath $target -PathType Leaf) {
                Remove-Item -LiteralPath $target -Force
            }
        }
        throw $installError
    }
    return $backupRoot
}

function Restore-MillenniumEye {
    param([string]$GameRoot, [string]$BackupRoot)
    Assert-MillenniumEyeClosed
    $gamePath = [IO.Path]::GetFullPath($GameRoot).TrimEnd([char[]]'\/')
    $backupPath = [IO.Path]::GetFullPath($BackupRoot)
    $prefix = $gamePath + [IO.Path]::DirectorySeparatorChar
    if (-not $backupPath.StartsWith($prefix, [StringComparison]::OrdinalIgnoreCase)) { throw 'Unsafe path for the backup.' }
    $relative = $backupPath.Substring($prefix.Length).Replace('\','/')
    if ($relative -notmatch '^UserData/MillenniumEye/Backups/[^/]+$') { throw 'Unsafe path for the backup.' }
    $null = Get-MillenniumEyePath $GameRoot $relative
    $receipt = Get-Content -LiteralPath (Join-Path $BackupRoot 'receipt.json') -Raw | ConvertFrom-Json
    foreach ($entry in $receipt.files) {
        if ($entry.path -notmatch '^(Mods/BlindDuel\.dll|Tolk\.dll|nvdaControllerClient64\.dll|SDL3\.dll|Accessibility/MillenniumEye/.+)$') { throw 'Unsafe path in the restore receipt.' }
        $target = Get-MillenniumEyePath $GameRoot $entry.path
        if (Test-Path -LiteralPath $target) {
            if (-not (Test-Path -LiteralPath $target -PathType Leaf)) {
                throw "Installed file changed; it was preserved: $($entry.path)"
            }
            $currentHash = (Get-FileHash -LiteralPath $target).Hash
            if ($currentHash -ne $entry.installedSha256 -and -not ($entry.hadOriginal -and $currentHash -eq $entry.originalSha256)) {
                throw "Installed file changed; it was preserved: $($entry.path)"
            }
        }
        if ($entry.hadOriginal) {
            $original = Get-MillenniumEyePath $BackupRoot ('originals/' + $entry.path)
            if ((Get-FileHash -LiteralPath $original).Hash -ne $entry.originalSha256) { throw 'Backup checksum failed; no files were restored.' }
        }
    }
    foreach ($entry in $receipt.files) {
        $target = Get-MillenniumEyePath $GameRoot $entry.path
        if ($entry.hadOriginal) {
            if ((Test-Path -LiteralPath $target -PathType Leaf) -and (Get-FileHash -LiteralPath $target).Hash -eq $entry.originalSha256) { continue }
            Copy-MillenniumEyeFile -Source (Get-MillenniumEyePath $BackupRoot ('originals/' + $entry.path)) -Destination $target
        } elseif (Test-Path -LiteralPath $target -PathType Leaf) {
            Remove-Item -LiteralPath $target -Force
        }
    }
}
