# Publishes PharmacyService.API then mirrors output to PROD deploy folder.
# Stop the Pharmacy site / app pool (or any dotnet process using that folder) first if robocopy reports "file in use".
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$csproj = Join-Path $repoRoot "HealthcarePlatform/PharmacyService/PharmacyService.API/PharmacyService.API.csproj"
$staging = Join-Path $repoRoot "artifacts/PharmacyService-Release"
$target = "I:\Projects\PROD\TriVita\services\PharmacyService"

if (-not (Test-Path $csproj)) {
  Write-Error "Project not found: $csproj"
  exit 1
}

New-Item -ItemType Directory -Force -Path $staging | Out-Null
New-Item -ItemType Directory -Force -Path $target | Out-Null

Write-Host "Publishing to $staging ..."
dotnet publish $csproj -c Release -o $staging
if ($LASTEXITCODE -ne 0) { exit $LASTEXITCODE }

Write-Host "Copying to $target ..."
robocopy $staging $target /MIR /R:2 /W:2
$code = $LASTEXITCODE
if ($code -ge 8) {
  Write-Error "robocopy failed with exit code $code"
  exit $code
}
Write-Host "Done (robocopy exit $code)."
