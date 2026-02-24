# FullCalendar Windows Service - Verwijder Script
# Run als Administrator!

param(
    [Parameter(Mandatory=$false)]
    [string]$ServiceName = "FullCalendarAgenda"
)

Write-Host "🗑️  FullCalendar Windows Service Uninstaller" -ForegroundColor Cyan
Write-Host "============================================" -ForegroundColor Cyan
Write-Host ""

# Check Administrator privileges
$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "❌ Error: Run this script as Administrator!" -ForegroundColor Red
    exit 1
}

# Check if service exists
$service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if (-not $service) {
    Write-Host "⚠️  Service '$ServiceName' not found" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Available services with 'FullCalendar' in the name:" -ForegroundColor Gray
    Get-Service | Where-Object { $_.Name -like "*FullCalendar*" -or $_.DisplayName -like "*FullCalendar*" } | Format-Table Name, DisplayName, Status
    exit 0
}

Write-Host "📋 Service Information:" -ForegroundColor Yellow
Write-Host "   Name:         $($service.Name)" -ForegroundColor Gray
Write-Host "   Display Name: $($service.DisplayName)" -ForegroundColor Gray
Write-Host "   Status:       $($service.Status)" -ForegroundColor Gray
Write-Host ""

# Confirm deletion
$confirmation = Read-Host "Are you sure you want to remove this service? (y/n)"
if ($confirmation -ne 'y') {
    Write-Host "❌ Cancelled by user" -ForegroundColor Yellow
    exit 0
}

# Stop the service
if ($service.Status -eq "Running") {
    Write-Host "⏹️  Stopping service..." -ForegroundColor Yellow
    Stop-Service -Name $ServiceName -Force
    Start-Sleep -Seconds 2
    Write-Host "✅ Service stopped" -ForegroundColor Green
}

# Delete the service
Write-Host "🗑️  Removing service..." -ForegroundColor Yellow
sc.exe delete $ServiceName

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Service removed successfully" -ForegroundColor Green
} else {
    Write-Host "❌ Failed to remove service (error code: $LASTEXITCODE)" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "✅ Uninstallation Complete!" -ForegroundColor Green
Write-Host ""
