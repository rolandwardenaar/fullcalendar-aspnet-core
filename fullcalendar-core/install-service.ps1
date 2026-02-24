# FullCalendar Windows Service - Installatie Script
# Run als Administrator!

param(
    [Parameter(Mandatory=$false)]
    [string]$ServiceName = "FullCalendarAgenda",
    
    [Parameter(Mandatory=$false)]
    [string]$DisplayName = "FullCalendar Agenda Service",
    
    [Parameter(Mandatory=$false)]
    [string]$Port = "8086"
)

Write-Host "🔧 FullCalendar Windows Service Installer" -ForegroundColor Cyan
Write-Host "=========================================" -ForegroundColor Cyan
Write-Host ""

# Check Administrator privileges
$currentPrincipal = New-Object Security.Principal.WindowsPrincipal([Security.Principal.WindowsIdentity]::GetCurrent())
if (-not $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)) {
    Write-Host "❌ Error: Run this script as Administrator!" -ForegroundColor Red
    exit 1
}

# Check if we're in the right directory
if (-not (Test-Path "fullcalendar-core.csproj")) {
    Write-Host "❌ Error: Run this script from the fullcalendar-core directory" -ForegroundColor Red
    exit 1
}

# Step 1: NPM Build
Write-Host "📦 Step 1/4: Building frontend assets..." -ForegroundColor Yellow
npm run build
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ NPM build failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Frontend build complete" -ForegroundColor Green
Write-Host ""

# Step 2: .NET Publish
Write-Host "🔨 Step 2/4: Publishing .NET application..." -ForegroundColor Yellow
dotnet publish -c Release -r win-x64 --self-contained -o ./publish
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ .NET publish failed!" -ForegroundColor Red
    exit 1
}
Write-Host "✅ .NET publish complete" -ForegroundColor Green
Write-Host ""

# Step 3: Stop and remove existing service if it exists
Write-Host "🧹 Step 3/4: Checking for existing service..." -ForegroundColor Yellow
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

# Step 4: Install as Windows Service
Write-Host "⚙️  Step 4/4: Installing Windows Service..." -ForegroundColor Yellow

$publishPath = Resolve-Path ".\publish"
$exePath = Join-Path $publishPath "fullcalendarcore.exe"

if (-not (Test-Path $exePath)) {
    Write-Host "❌ Error: Executable not found at $exePath" -ForegroundColor Red
    exit 1
}

# Create the service
sc.exe create $ServiceName binPath= $exePath start= auto DisplayName= $DisplayName

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Service creation failed!" -ForegroundColor Red
    exit 1
}

# Set environment variables for the service
$regPath = "HKLM:\SYSTEM\CurrentControlSet\Services\$ServiceName"
$envVars = @(
    "ASPNETCORE_URLS=http://localhost:$Port",
    "ASPNETCORE_ENVIRONMENT=Production"
)
Set-ItemProperty -Path $regPath -Name "Environment" -Value $envVars -Type MultiString

# Set service description
sc.exe description $ServiceName "ASP.NET Core Kestrel web server voor FullCalendar Agenda op poort $Port"

# Set service to restart on failure
sc.exe failure $ServiceName reset= 86400 actions= restart/60000/restart/60000/restart/60000

Write-Host "✅ Windows Service installed successfully" -ForegroundColor Green
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
Write-Host "✅ Installation Complete!" -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""
Write-Host "Service Name:    $ServiceName" -ForegroundColor White
Write-Host "Display Name:    $DisplayName" -ForegroundColor White
Write-Host "URL:             http://localhost:$Port" -ForegroundColor White
Write-Host "Public URL:      https://agenda.wardenaar.org" -ForegroundColor White
Write-Host "Status:          $($service.Status)" -ForegroundColor White
Write-Host ""
Write-Host "Management Commands:" -ForegroundColor Yellow
Write-Host "  Start:   Start-Service $ServiceName" -ForegroundColor Gray
Write-Host "  Stop:    Stop-Service $ServiceName" -ForegroundColor Gray
Write-Host "  Restart: Restart-Service $ServiceName" -ForegroundColor Gray
Write-Host "  Status:  Get-Service $ServiceName" -ForegroundColor Gray
Write-Host "  Logs:    Get-EventLog -LogName Application -Source $ServiceName -Newest 50" -ForegroundColor Gray
Write-Host ""
