# FullCalendar Windows Service - Beheer Script
# Snel beheer van de Windows Service

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("status", "start", "stop", "restart", "logs")]
    [string]$Action = "status",
    
    [Parameter(Mandatory=$false)]
    [string]$ServiceName = "FullCalendarAgenda"
)

function Show-ServiceStatus {
    $service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
    
    if (-not $service) {
        Write-Host "❌ Service '$ServiceName' not found" -ForegroundColor Red
        return
    }
    
    Write-Host ""
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
    Write-Host "📊 Service Status" -ForegroundColor Cyan
    Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
    
    $statusColor = switch ($service.Status) {
        "Running" { "Green" }
        "Stopped" { "Red" }
        default { "Yellow" }
    }
    
    Write-Host "Name:         " -NoNewline -ForegroundColor Gray
    Write-Host $service.Name -ForegroundColor White
    
    Write-Host "Display Name: " -NoNewline -ForegroundColor Gray
    Write-Host $service.DisplayName -ForegroundColor White
    
    Write-Host "Status:       " -NoNewline -ForegroundColor Gray
    Write-Host $service.Status -ForegroundColor $statusColor
    
    Write-Host "Start Type:   " -NoNewline -ForegroundColor Gray
    Write-Host $service.StartType -ForegroundColor White
    
    Write-Host ""
    
    if ($service.Status -eq "Running") {
        Write-Host "🌐 Application URLs:" -ForegroundColor Cyan
        Write-Host "   Local:  http://localhost:8086" -ForegroundColor Gray
        Write-Host "   Public: https://agenda.wardenaar.org" -ForegroundColor Gray
        Write-Host ""
    }
}

# Check Administrator privileges for non-status commands
if ($Action -ne "status" -and $Action -ne "logs") {
    $currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
    if (-not $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
        Write-Host "❌ Error: Run this script as Administrator for this action!" -ForegroundColor Red
        exit 1
    }
}

switch ($Action) {
    "status" {
        Show-ServiceStatus
    }
    
    "start" {
        Write-Host "🚀 Starting service..." -ForegroundColor Yellow
        Start-Service -Name $ServiceName
        Start-Sleep -Seconds 2
        Show-ServiceStatus
    }
    
    "stop" {
        Write-Host "⏹️  Stopping service..." -ForegroundColor Yellow
        Stop-Service -Name $ServiceName -Force
        Start-Sleep -Seconds 2
        Show-ServiceStatus
    }
    
    "restart" {
        Write-Host "🔄 Restarting service..." -ForegroundColor Yellow
        Restart-Service -Name $ServiceName -Force
        Start-Sleep -Seconds 3
        Show-ServiceStatus
    }
    
    "logs" {
        Write-Host "📜 Recent logs (last 20 entries):" -ForegroundColor Cyan
        Write-Host ""
        Get-EventLog -LogName Application -Source ".NET Runtime" -Newest 20 | 
            Where-Object { $_.Message -like "*fullcalendar*" } |
            Format-Table TimeGenerated, EntryType, Message -AutoSize
        Write-Host ""
        Write-Host "💡 Tip: Open Event Viewer for detailed logs" -ForegroundColor Gray
    }
}
