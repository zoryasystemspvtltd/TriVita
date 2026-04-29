param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]] $EFArgs
)
$ErrorActionPreference = 'Stop'
Set-Location $PSScriptRoot
$Conn = 'Server=.\SQLEXPRESS;Database=TriVita;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=true'
if (-not $EFArgs -or $EFArgs.Count -eq 0) {
    Write-Error 'Example: .\run-ef.ps1 database update'
}
& dotnet ef @EFArgs --project TriVita.UnifiedDatabase.csproj --context HealthcareDbContext --connection $Conn
