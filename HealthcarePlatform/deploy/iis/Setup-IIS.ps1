<#
.SYNOPSIS
    Creates IIS app pools, parent site TriVita-API (port 80), child applications, and TriVita-Portal (port 3000).
    Requires Administrator PowerShell and IIS + ASP.NET Core Hosting Bundle + URL Rewrite.

.PARAMETER ProdRoot
    Must match Publish-TriVita.ps1 output. Default: I:\Projects\PROD\TriVita
#>
#Requires -RunAsAdministrator
[CmdletBinding()]
param(
    [string] $ProdRoot = 'I:\Projects\PROD\TriVita'
)

$ErrorActionPreference = 'Stop'

if (-not (Get-Module -ListAvailable -Name WebAdministration)) {
    throw 'IIS WebAdministration module not found. Enable IIS Management Scripts and Tools.'
}
Import-Module WebAdministration

function Ensure-AppPool([string] $Name) {
    if (-not (Test-Path "IIS:\AppPools\$Name")) {
        Write-Host "Creating app pool: $Name"
        New-WebAppPool -Name $Name | Out-Null
    }
    Set-ItemProperty "IIS:\AppPools\$Name" -Name managedRuntimeVersion -Value ''
    Set-ItemProperty "IIS:\AppPools\$Name" -Name managedPipelineMode -Value 'Integrated'
    Start-WebAppPool -Name $Name
}

$apiSite = 'TriVita-API'
$portalSite = 'TriVita-Portal'

$pools = @(
    @{ Pool = 'TriVita-HMS';            App = 'hms';           Path = 'services\HMSService' },
    @{ Pool = 'TriVita-LMS';            App = 'lms';           Path = 'services\LMSService' },
    @{ Pool = 'TriVita-LIS';            App = 'lis';           Path = 'services\LISService' },
    @{ Pool = 'TriVita-Pharmacy';       App = 'pharmacy';      Path = 'services\PharmacyService' },
    @{ Pool = 'TriVita-Shared';         App = 'shared';        Path = 'services\SharedService' },
    @{ Pool = 'TriVita-Identity';       App = 'identity';      Path = 'services\IdentityService' },
    @{ Pool = 'TriVita-Communication'; App = 'communication'; Path = 'services\CommunicationService' }
)

foreach ($p in $pools) { Ensure-AppPool $p.Pool }

$apiRoot = Join-Path $ProdRoot 'api-root'
if (-not (Test-Path $apiRoot)) { New-Item -ItemType Directory -Force -Path $apiRoot | Out-Null }

if (-not (Get-Website -Name $apiSite -ErrorAction SilentlyContinue)) {
    Write-Host "Creating website $apiSite on port 80"
    New-Website -Name $apiSite -PhysicalPath $apiRoot -Port 80 -ApplicationPool 'DefaultAppPool' | Out-Null
}
else {
    Write-Host "Website $apiSite already exists; skipping New-Website"
}

foreach ($p in $pools) {
    $phys = Join-Path $ProdRoot $p.Path
    if (-not (Test-Path $phys)) { throw "Missing published output: $phys. Run Publish-TriVita.ps1 first." }
    if (Get-WebApplication -Site $apiSite -Name $p.App -ErrorAction SilentlyContinue) {
        Write-Host "Recreating app /$($p.App) (refresh paths and pool)"
        Remove-WebApplication -Site $apiSite -Name $p.App
    }
    Write-Host "Creating app /$($p.App) -> $phys (pool $($p.Pool))"
    New-WebApplication -Site $apiSite -Name $p.App -PhysicalPath $phys -ApplicationPool $p.Pool | Out-Null
}

$portalPath = Join-Path $ProdRoot 'portal'
if (-not (Test-Path (Join-Path $portalPath 'index.html'))) {
    throw "Portal not built: missing $portalPath\index.html. Run Publish-TriVita.ps1."
}

if (-not (Get-Website -Name $portalSite -ErrorAction SilentlyContinue)) {
    Write-Host "Creating website $portalSite on port 3000"
    New-Website -Name $portalSite -PhysicalPath $portalPath -Port 3000 -ApplicationPool 'DefaultAppPool' | Out-Null
}
else {
    Write-Host "Website $portalSite already exists; set physical path manually if needed"
    Set-ItemProperty "IIS:\Sites\$portalSite" -Name physicalPath -Value $portalPath
}

Write-Host 'IIS setup complete.' -ForegroundColor Green
Write-Host 'Portal: http://localhost:3000' -ForegroundColor Green
Write-Host 'APIs:   http://localhost/hms|lms|lis|pharmacy|shared|identity|communication' -ForegroundColor Green
Write-Host 'If port 80 is in use, stop Default Web Site or change TriVita-API binding.' -ForegroundColor Yellow
