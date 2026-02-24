# FullCalendar - Deploy naar Webserver
# Publish lokaal en kopieer naar webserver

param(
    [Parameter(Mandatory=$false)]
    [string]$WebServerPath = "\\192.168.1.229\c$\inetpub\fullcalendar",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBuild,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipNpm,
    
    [Parameter(Mandatory=$false)]
    [switch]$RestartService
)

Write-Host "🚀 FullCalendar Deploy to Webserver" -ForegroundColor Cyan
Write-Host "====================================" -ForegroundColor Cyan
Write-Host ""

# Check if we're in the right directory
if (-not (Test-Path "fullcalendar-core.csproj")) {
    Write-Host "❌ Error: Run this script from the fullcalendar-core directory" -ForegroundColor Red
    exit 1
}

# Step 1: NPM Build
if (-not $SkipNpm) {
    Write-Host "📦 Step 1/3: Building frontend assets..." -ForegroundColor Yellow
    npm run build
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ NPM build failed!" -ForegroundColor Red
        exit 1
    }
    Write-Host "✅ Frontend build complete" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host "⏭️  Skipping NPM build" -ForegroundColor Gray
    Write-Host ""
}

# Step 2: .NET Publish
if (-not $SkipBuild) {
    Write-Host "🔨 Step 2/3: Publishing .NET application..." -ForegroundColor Yellow
    dotnet publish -c Release -r win-x64 --self-contained -o ./publish
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ .NET publish failed!" -ForegroundColor Red
        exit 1
    }
    Write-Host "✅ .NET publish complete" -ForegroundColor Green
    Write-Host ""
} else {
    Write-Host "⏭️  Skipping .NET build" -ForegroundColor Gray
    Write-Host ""
}

# Step 3: Copy to Webserver
Write-Host "📤 Step 3/3: Copying to webserver..." -ForegroundColor Yellow

# Check if webserver path is accessible
if (-not (Test-Path $WebServerPath)) {
    Write-Host "⚠️  Webserver path not accessible: $WebServerPath" -ForegroundColor Yellow
    Write-Host "   Creating directory..." -ForegroundColor Gray
    try {
        New-Item -Path $WebServerPath -ItemType Directory -Force | Out-Null
    } catch {
        Write-Host "❌ Cannot access webserver path!" -ForegroundColor Red
        Write-Host "   Make sure you have network access to: $WebServerPath" -ForegroundColor Yellow
        Write-Host "   Or provide a different path with: -WebServerPath 'C:\path'" -ForegroundColor Yellow
        exit 1
    }
}

# Stop service on webserver if requested
if ($RestartService) {
    Write-Host "   Stopping service on webserver..." -ForegroundColor Gray
    Invoke-Command -ComputerName "192.168.1.229" -ScriptBlock {
        Stop-Service -Name "FullCalendarAgenda" -Force -ErrorAction SilentlyContinue
    } -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
}

# Copy files
$publishPath = ".\publish\*"
Write-Host "   Copying files to: $WebServerPath\publish" -ForegroundColor Gray

try {
    # Ensure publish folder exists on webserver
    $destPublish = Join-Path $WebServerPath "publish"
    if (-not (Test-Path $destPublish)) {
        New-Item -Path $destPublish -ItemType Directory -Force | Out-Null
    }
    
    # Copy all files
    Copy-Item -Path $publishPath -Destination $destPublish -Recurse -Force
    
    # Copy management scripts
    Copy-Item -Path "register-service.ps1" -Destination $WebServerPath -Force -ErrorAction SilentlyContinue
    Copy-Item -Path "uninstall-service.ps1" -Destination $WebServerPath -Force -ErrorAction SilentlyContinue
    Copy-Item -Path "manage-service.ps1" -Destination $WebServerPath -Force -ErrorAction SilentlyContinue
    
    Write-Host "✅ Files copied successfully" -ForegroundColor Green
} catch {
    Write-Host "❌ Copy failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Restart service on webserver if requested
if ($RestartService) {
    Write-Host "   Starting service on webserver..." -ForegroundColor Gray
    Invoke-Command -ComputerName "192.168.1.229" -ScriptBlock {
        Start-Service -Name "FullCalendarAgenda" -ErrorAction SilentlyContinue
    } -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
}

Write-Host ""
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host "✅ Deployment Complete!" -ForegroundColor Green
Write-Host "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━" -ForegroundColor Cyan
Write-Host ""
Write-Host "Files deployed to: $WebServerPath\publish" -ForegroundColor White
Write-Host ""

if (-not $RestartService) {
    Write-Host "⚠️  Next Steps:" -ForegroundColor Yellow
    Write-Host "   1. RDP/SSH to webserver (192.168.1.229)" -ForegroundColor Gray
    Write-Host "   2. Run: cd $WebServerPath" -ForegroundColor Gray
    Write-Host "   3. Run: .\register-service.ps1" -ForegroundColor Gray
    Write-Host ""
} else {
    Write-Host "✅ Service has been restarted on webserver" -ForegroundColor Green
    Write-Host ""
}
