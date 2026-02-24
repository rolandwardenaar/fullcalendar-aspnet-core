# FullCalendar Service Diagnostics
# Troubleshoot service issues

param(
    [Parameter(Mandatory=$false)]
    [string]$ServiceName = "FullCalendarAgenda",
    
    [Parameter(Mandatory=$false)]
    [int]$Port = 8086
)

Write-Host ""
Write-Host "🔍 FullCalendar Service Diagnostics" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
Write-Host ""

# 1. Check Service Status
Write-Host "📊 1. Service Status" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

$service = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue

if (-not $service) {
    Write-Host "❌ Service '$ServiceName' not found!" -ForegroundColor Red
    Write-Host "   Run register-service.ps1 first" -ForegroundColor Yellow
    exit 1
}

$statusColor = switch ($service.Status) {
    "Running" { "Green" }
    "Stopped" { "Red" }
    default { "Yellow" }
}

Write-Host "   Name:         $($service.Name)" -ForegroundColor White
Write-Host "   Display Name: $($service.DisplayName)" -ForegroundColor White
Write-Host "   Status:       " -NoNewline
Write-Host $service.Status -ForegroundColor $statusColor
Write-Host "   Start Type:   $($service.StartType)" -ForegroundColor White
Write-Host ""

# 2. Check if port is listening
Write-Host "📡 2. Port $Port Status" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

$portListener = Get-NetTCPConnection -LocalPort $Port -ErrorAction SilentlyContinue | Select-Object -First 1

if ($portListener) {
    Write-Host "   ✅ Port $Port is LISTENING" -ForegroundColor Green
    Write-Host "   Process ID: $($portListener.OwningProcess)" -ForegroundColor White
    
    $process = Get-Process -Id $portListener.OwningProcess -ErrorAction SilentlyContinue
    if ($process) {
        Write-Host "   Process:    $($process.ProcessName)" -ForegroundColor White
        Write-Host "   Path:       $($process.Path)" -ForegroundColor Gray
    }
} else {
    Write-Host "   ❌ Port $Port is NOT listening!" -ForegroundColor Red
    Write-Host "   The application is not running or not bound to this port" -ForegroundColor Yellow
}
Write-Host ""

# 3. Test localhost connection
Write-Host "🌐 3. Localhost Connection Test" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

try {
    $response = Invoke-WebRequest -Uri "http://localhost:$Port" -UseBasicParsing -TimeoutSec 5 -ErrorAction Stop
    Write-Host "   ✅ HTTP $($response.StatusCode) - Application is responding!" -ForegroundColor Green
} catch {
    Write-Host "   ❌ Connection failed!" -ForegroundColor Red
    Write-Host "   Error: $($_.Exception.Message)" -ForegroundColor Yellow
}
Write-Host ""

# 4. Check Cloudflared Tunnel
Write-Host "🔗 4. Cloudflared Tunnel Status" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

$cloudflaredService = Get-Service -Name "Cloudflared" -ErrorAction SilentlyContinue
if ($cloudflaredService) {
    $cfStatusColor = if ($cloudflaredService.Status -eq "Running") { "Green" } else { "Red" }
    Write-Host "   Status: " -NoNewline
    Write-Host $cloudflaredService.Status -ForegroundColor $cfStatusColor
    
    if ($cloudflaredService.Status -eq "Running") {
        # Try to get tunnel info
        $tunnelInfo = cloudflared tunnel list 2>&1 | Out-String
        if ($tunnelInfo -match "CONNECTIONS") {
            Write-Host "   Tunnel service is active" -ForegroundColor Green
        }
    }
} else {
    Write-Host "   ⚠️  Cloudflared service not found" -ForegroundColor Yellow
}
Write-Host ""

# 5. Recent Event Logs
Write-Host "📜 5. Recent Error Logs (last 10 errors)" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

$recentErrors = Get-EventLog -LogName Application -EntryType Error -Newest 10 -ErrorAction SilentlyContinue | 
    Where-Object { $_.Source -like "*.NET*" -or $_.Message -like "*fullcalendar*" -or $_.Message -like "*$ServiceName*" }

if ($recentErrors) {
    foreach ($error in $recentErrors) {
        Write-Host "   [$($error.TimeGenerated)] $($error.Source)" -ForegroundColor Gray
        Write-Host "   $($error.Message.Substring(0, [Math]::Min(150, $error.Message.Length)))..." -ForegroundColor Red
        Write-Host ""
    }
} else {
    Write-Host "   ✅ No recent errors found" -ForegroundColor Green
}
Write-Host ""

# 6. Service Configuration
Write-Host "⚙️  6. Service Configuration" -ForegroundColor Yellow
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Gray

$regPath = "HKLM:\SYSTEM\CurrentControlSet\Services\$ServiceName"
if (Test-Path $regPath) {
    $imagePath = (Get-ItemProperty -Path $regPath).ImagePath
    Write-Host "   Executable: $imagePath" -ForegroundColor White
    
    $envVars = (Get-ItemProperty -Path $regPath -Name "Environment" -ErrorAction SilentlyContinue).Environment
    if ($envVars) {
        Write-Host "   Environment Variables:" -ForegroundColor White
        foreach ($env in $envVars) {
            Write-Host "     - $env" -ForegroundColor Gray
        }
    } else {
        Write-Host "   ⚠️  No environment variables configured!" -ForegroundColor Yellow
    }
}
Write-Host ""

# Summary and recommendations
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "📋 Summary & Recommendations" -ForegroundColor Cyan
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""

if ($service.Status -ne "Running") {
    Write-Host "⚠️  Service is not running!" -ForegroundColor Yellow
    Write-Host "   Try: Start-Service $ServiceName" -ForegroundColor Gray
    Write-Host ""
}

if (-not $portListener -and $service.Status -eq "Running") {
    Write-Host "⚠️  Service is running but not listening on port $Port" -ForegroundColor Yellow
    Write-Host "   Possible issues:" -ForegroundColor Gray
    Write-Host "   - Environment variables not set correctly" -ForegroundColor Gray
    Write-Host "   - Application crashed on startup" -ForegroundColor Gray
    Write-Host "   - Check Event Viewer for detailed errors" -ForegroundColor Gray
    Write-Host ""
    Write-Host "   Try:" -ForegroundColor Yellow
    Write-Host "   1. Stop-Service $ServiceName" -ForegroundColor Gray
    Write-Host "   2. Check: eventvwr.msc -> Windows Logs -> Application" -ForegroundColor Gray
    Write-Host "   3. Re-register: .\register-service.ps1" -ForegroundColor Gray
    Write-Host ""
}

if ($portListener -and $cloudflaredService.Status -eq "Running") {
    Write-Host "✅ Everything looks good!" -ForegroundColor Green
    Write-Host "   Local URL:  http://localhost:$Port" -ForegroundColor White
    Write-Host "   Public URL: https://agenda.wardenaar.org" -ForegroundColor White
    Write-Host ""
}

Write-Host "💡 Useful Commands:" -ForegroundColor Cyan
Write-Host "   Restart service:    Restart-Service $ServiceName" -ForegroundColor Gray
Write-Host "   View logs:          Get-EventLog -LogName Application -Newest 20" -ForegroundColor Gray
Write-Host "   Re-register:        .\register-service.ps1" -ForegroundColor Gray
Write-Host "   Test manually:      cd publish; .\fullcalendarcore.exe" -ForegroundColor Gray
Write-Host ""
