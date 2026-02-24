# FullCalendar Windows Service - Registratie Script (voor webserver)
# Run op de WEBSERVER als Administrator!
# Zorg dat de publish folder al op de webserver staat

param(
    [Parameter(Mandatory=$false)]
    [string]$ServiceName = "FullCalendarAgenda",
    
    [Parameter(Mandatory=$false)]
    [string]$DisplayName = "FullCalendar Agenda Service",
    
    [Parameter(Mandatory=$false)]
    [string]$Port = "8086",
    
    [Parameter(Mandatory=$false)]
    [string]$PublishPath = ".\publish"
)

Write-Host "🔧 FullCalendar Windows Service Registration" -ForegroundColor Cyan
Write-Host "=============================================" -ForegroundColor Cyan
Write-Host ""

# Check Administrator privileges
$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "❌ Error: Run this script as Administrator!" -ForegroundColor Red
    exit 1
}

# Check if publish folder exists
if (-not (Test-Path $PublishPath)) {
    Write-Host "❌ Error: Publish folder not found at: $PublishPath" -ForegroundColor Red
    Write-Host "   Make sure you copied the published files to this server first!" -ForegroundColor Yellow
    exit 1
}

# Check if executable exists
$publishPathResolved = Resolve-Path $PublishPath
$exePath = Join-Path $publishPathResolved "fullcalendarcore.exe"

if (-not (Test-Path $exePath)) {
    Write-Host "❌ Error: Executable not found at $exePath" -ForegroundColor Red
    Write-Host "   Make sure the publish folder contains fullcalendarcore.exe" -ForegroundColor Yellow
    exit 1
}

Write-Host "📋 Configuration:" -ForegroundColor Yellow
Write-Host "   Service Name:  $ServiceName" -ForegroundColor Gray
Write-Host "   Display Name:  $DisplayName" -ForegroundColor Gray
Write-Host "   Executable:    $exePath" -ForegroundColor Gray
Write-Host "   Port:          $Port" -ForegroundColor Gray
Write-Host ""

# Stop and remove existing service if it exists
Write-Host "🧹 Step 1/2: Checking for existing service..." -ForegroundColor Yellow
$existingService = Get-Service -Name $ServiceName -ErrorAction SilentlyContinue
if ($existingService) {
    Write-Host "   Found existing service, stopping and removing..." -ForegroundColor Gray
    Stop-Service -Name $ServiceName -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
    sc.exe delete $ServiceName
    Start-Sleep -Seconds 2
    Write-Host "✅ Existing service removed" -ForegroundColor Green
} else {
    Write-Host "   No existing service found" -ForegroundColor Gray
}
Write-Host ""

# Install as Windows Service
Write-Host "⚙️  Step 2/2: Installing Windows Service..." -ForegroundColor Yellow

# Create the service with environment variables in binPath
$binPathWithEnv = "$exePath"

sc.exe create $ServiceName binPath= $binPathWithEnv start= auto DisplayName= $DisplayName

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Service creation failed!" -ForegroundColor Red
    exit 1
}

# Set environment variables for the service (multiple methods for compatibility)
$regPath = "HKLM:\SYSTEM\CurrentControlSet\Services\$ServiceName"

# Method 1: Registry Environment (MultiString)
# Listen on all interfaces (0.0.0.0) so Cloudflare Tunnel can connect
$envVars = @(
    "ASPNETCORE_URLS=http://0.0.0.0:$Port",
    "ASPNETCORE_ENVIRONMENT=Production",
    "DOTNET_ENVIRONMENT=Production"
)
New-ItemProperty -Path $regPath -Name "Environment" -Value $envVars -PropertyType MultiString -Force | Out-Null

# Method 2: Create a environment file that the service can read
$envFilePath = Join-Path $publishPathResolved "service.env"
@"
ASPNETCORE_URLS=http://0.0.0.0:$Port
ASPNETCORE_ENVIRONMENT=Production
DOTNET_ENVIRONMENT=Production
"@ | Out-File -FilePath $envFilePath -Encoding UTF8 -Force

Write-Host "   Environment variables configured" -ForegroundColor Gray

# Set service description
sc.exe description $ServiceName "ASP.NET Core Kestrel web server voor FullCalendar Agenda op poort $Port"

# Set service to restart on failure
sc.exe failure $ServiceName reset= 86400 actions= restart/60000/restart/60000/restart/60000

Write-Host "✅ Windows Service registered successfully" -ForegroundColor Green
Write-Host ""

# Start the service
Write-Host "🚀 Starting service..." -ForegroundColor Yellow
Start-Service -Name $ServiceName

Start-Sleep -Seconds 3

$service = Get-Service -Name $ServiceName
if ($service.Status -eq "Running") {
    Write-Host "✅ Service is running!" -ForegroundColor Green
} else {
    Write-Host "⚠️  Service status: $($service.Status)" -ForegroundColor Yellow
    Write-Host "   Check Event Viewer for errors" -ForegroundColor Gray
}

Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "✅ Registration Complete!" -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""
Write-Host "Service Name:    $ServiceName" -ForegroundColor White
Write-Host "Display Name:    $DisplayName" -ForegroundColor White
Write-Host "Executable:      $exePath" -ForegroundColor White
Write-Host "URL:             http://localhost:$Port" -ForegroundColor White
Write-Host "Public URL:      https://agenda.wardenaar.org" -ForegroundColor White
Write-Host "Status:          $($service.Status)" -ForegroundColor White
Write-Host ""
Write-Host "Management Commands:" -ForegroundColor Yellow
Write-Host "  Start:   Start-Service $ServiceName" -ForegroundColor Gray
Write-Host "  Stop:    Stop-Service $ServiceName" -ForegroundColor Gray
Write-Host "  Restart: Restart-Service $ServiceName" -ForegroundColor Gray
Write-Host "  Status:  Get-Service $ServiceName" -ForegroundColor Gray
Write-Host ""
