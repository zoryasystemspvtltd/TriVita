<#
.SYNOPSIS
    Publishes all TriVita API hosts (excludes TriVita.UnifiedDatabase) and builds the React portal to I:\Projects\PROD\TriVita.

.PARAMETER ProdRoot
    Output root. Default: I:\Projects\PROD\TriVita

.PARAMETER RepoRoot
    Repository root containing HealthcarePlatform and triVita-portal. Default: two levels above this script (TriVita repo root).
#>
[CmdletBinding()]
param(
    [string] $ProdRoot = 'I:\Projects\PROD\TriVita',
    [string] $RepoRoot = ''
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

New-Item -ItemType Directory -Force -Path (Join-Path $ProdRoot 'services') | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $ProdRoot 'portal') | Out-Null
New-Item -ItemType Directory -Force -Path (Join-Path $ProdRoot 'api-root') | Out-Null

Write-Host '=== dotnet publish APIs (Release) ===' -ForegroundColor Cyan
foreach ($s in $services) {
    $csproj = Join-Path $Platform $s.Project
    $out = Join-Path $ProdRoot ('services\' + $s.Name)
    if (-not (Test-Path $csproj)) { throw "Missing project: $csproj" }
    Write-Host "Publishing $($s.Name) -> $out"
    dotnet publish $csproj -c Release -o $out --no-self-contained
    if ($LASTEXITCODE -ne 0) { throw "Publish failed: $($s.Name)" }
}

Write-Host '=== npm build portal ===' -ForegroundColor Cyan
if (-not (Test-Path $Portal)) { throw "Missing portal folder: $Portal" }
Push-Location $Portal
try {
    npm install
    if ($LASTEXITCODE -ne 0) { throw 'npm install failed' }
    npm run build
    if ($LASTEXITCODE -ne 0) { throw 'npm run build failed' }
}
finally {
    Pop-Location
}

$dist = Join-Path $Portal 'dist'
if (-not (Test-Path $dist)) { throw "Missing dist folder: $dist" }

Write-Host "Copying dist -> $(Join-Path $ProdRoot 'portal')" -ForegroundColor Cyan
Get-ChildItem (Join-Path $ProdRoot 'portal') -ErrorAction SilentlyContinue | Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
Copy-Item -Path (Join-Path $dist '*') -Destination (Join-Path $ProdRoot 'portal') -Recurse -Force

$readme = Join-Path $ProdRoot 'api-root\README.txt'
Set-Content -Path $readme -Value 'Placeholder physical path for IIS parent site TriVita-API. Applications map /hms, /lms, etc. to ..\services\...'

Write-Host "Done. Output: $ProdRoot" -ForegroundColor Green
