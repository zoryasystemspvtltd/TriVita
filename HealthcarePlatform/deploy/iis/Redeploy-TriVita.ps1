<#
.SYNOPSIS
    Rebuilds and republishes TriVita APIs (and optional portal) to existing I:\Projects\PROD\TriVita folders.
    Does not delete folders, change IIS, or touch TriVita.UnifiedDatabase.

.PARAMETER SkipPortal
    If set, skips npm build and portal copy.

.PARAMETER SkipIisReset
    If set, does not run iisreset (requires Administrator).
#>
[CmdletBinding()]
param(
    [string] $ProdRoot = 'I:\Projects\PROD\TriVita',
    [string] $RepoRoot = '',
    [switch] $SkipPortal,
    [switch] $SkipIisReset
)

$ErrorActionPreference = 'Stop'

if ([string]::IsNullOrWhiteSpace($RepoRoot)) {
    $scriptRoot = if ($PSScriptRoot) { $PSScriptRoot } else { Split-Path -Parent $MyInvocation.MyCommand.Path }
    $RepoRoot = (Resolve-Path (Join-Path $scriptRoot '..\..\..')).Path
}

$Platform = Join-Path $RepoRoot 'HealthcarePlatform'
$Portal = Join-Path $RepoRoot 'triVita-portal'

$services = @(
    @{ Name = 'HMSService';      Project = 'HMSService\HMSService.API\HMSService.API.csproj' },
    @{ Name = 'LMSService';      Project = 'LMSService\LMSService.API\LMSService.API.csproj' },
    @{ Name = 'LISService';      Project = 'LISService\LISService.API\LISService.API.csproj' },
    @{ Name = 'PharmacyService'; Project = 'PharmacyService\PharmacyService.API\PharmacyService.API.csproj' },
    @{ Name = 'SharedService';   Project = 'SharedService\SharedService.API\SharedService.API.csproj' },
    @{ Name = 'IdentityService'; Project = 'IdentityService\IdentityService.API\IdentityService.API.csproj' },
    @{ Name = 'CommunicationService'; Project = 'CommunicationService\CommunicationService.API\CommunicationService.API.csproj' }
)

$configFiles = @(
    'appsettings.json',
    'appsettings.Production.json',
    'appsettings.Local.json',
    'appsettings.Development.json'
)

function Backup-AppSettings([string] $serviceOutDir) {
    $saved = @{}
    if (-not (Test-Path $serviceOutDir)) { return $saved }
    foreach ($name in $configFiles) {
        $full = Join-Path $serviceOutDir $name
        if (Test-Path $full) {
            $saved[$name] = [System.IO.File]::ReadAllText($full)
        }
    }
    return $saved
}

function Restore-AppSettings([string] $serviceOutDir, [hashtable] $saved) {
    foreach ($kv in $saved.GetEnumerator()) {
        $dest = Join-Path $serviceOutDir $kv.Key
        $utf8NoBom = New-Object System.Text.UTF8Encoding $false
        [System.IO.File]::WriteAllText($dest, $kv.Value, $utf8NoBom)
        Write-Host "  Restored $($kv.Key) (preserved PROD customization)" -ForegroundColor DarkGray
    }
}

Write-Host '=== dotnet build + publish (Release) -> same output folders ===' -ForegroundColor Cyan
foreach ($s in $services) {
    $csproj = Join-Path $Platform $s.Project
    $out = Join-Path $ProdRoot ('services\' + $s.Name)
    if (-not (Test-Path $csproj)) { throw "Missing project: $csproj" }
    if (-not (Test-Path $out)) {
        throw "Expected output folder missing (do not delete per policy): $out"
    }

    Write-Host "`n[$($s.Name)]" -ForegroundColor Yellow
    $backup = Backup-AppSettings $out

    dotnet build $csproj -c Release
    if ($LASTEXITCODE -ne 0) { throw "Build failed: $($s.Name)" }

    dotnet publish $csproj -c Release -o $out --no-build
    if ($LASTEXITCODE -ne 0) { throw "Publish failed: $($s.Name)" }

    if ($backup.Count -gt 0) {
        Restore-AppSettings $out $backup
    }
}

if (-not $SkipPortal) {
    if (-not (Test-Path $Portal)) { Write-Warning "Portal repo path not found: $Portal - skipping portal." }
    else {
        $portalOut = Join-Path $ProdRoot 'portal'
        if (-not (Test-Path $portalOut)) { throw "Expected portal folder missing: $portalOut" }

        Write-Host "`n=== Portal (npm run build) ===" -ForegroundColor Cyan
        $portalBackup = @{}
        foreach ($name in @('web.config')) {
            $full = Join-Path $portalOut $name
            if (Test-Path $full) { $portalBackup[$name] = [System.IO.File]::ReadAllText($full) }
        }

        Push-Location $Portal
        try {
            npm install
            if ($LASTEXITCODE -ne 0) { throw 'npm install failed' }
            npm run build
            if ($LASTEXITCODE -ne 0) { throw 'npm run build failed' }
        }
        finally { Pop-Location }

        $dist = Join-Path $Portal 'dist'
        if (-not (Test-Path $dist)) { throw 'Missing triVita-portal\dist - run build from repo first.' }
        Copy-Item -Path (Join-Path $dist '*') -Destination $portalOut -Recurse -Force

        foreach ($kv in $portalBackup.GetEnumerator()) {
            $utf8NoBom = New-Object System.Text.UTF8Encoding $false
            [System.IO.File]::WriteAllText((Join-Path $portalOut $kv.Key), $kv.Value, $utf8NoBom)
            Write-Host "  Restored portal $($kv.Key)" -ForegroundColor DarkGray
        }
    }
}

if (-not $SkipIisReset) {
    Write-Host "`n=== iisreset ===" -ForegroundColor Cyan
    try {
        & iisreset 2>&1 | Out-Host
        if ($LASTEXITCODE -ne 0) { Write-Warning "iisreset exited $LASTEXITCODE - run PowerShell as Administrator if services did not recycle." }
    }
    catch {
        Write-Warning "Could not run iisreset: $_"
    }
}

Write-Host "`nRedeploy finished. $ProdRoot" -ForegroundColor Green
