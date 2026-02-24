# FullCalendar Deployment Script
# Publisht de applicatie en start Kestrel op poort 8086

param(
    [Parameter(Mandatory=$false)]
    [string]$Port = "8086",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBuild,
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipNpm
)

Write-Host "🚀 FullCalendar Deployment Script" -ForegroundColor Cyan
Write-Host "=================================" -ForegroundColor Cyan
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

# Step 3: Start Application
Write-Host "🏃 Step 3/3: Starting Kestrel server..." -ForegroundColor Yellow
Write-Host ""
Write-Host "📍 Server URL: http://localhost:$Port" -ForegroundColor Cyan
Write-Host "🌍 Environment: Production" -ForegroundColor Cyan
Write-Host "💾 Database: fullcalendar.db" -ForegroundColor Cyan
Write-Host ""
Write-Host "Press Ctrl+C to stop the server" -ForegroundColor Gray
Write-Host ""

# Set environment variables
$env:ASPNETCORE_URLS = "http://localhost:$Port"
$env:ASPNETCORE_ENVIRONMENT = "Production"

# Navigate to publish folder and start
Push-Location ./publish
try {
    # Start the application
    dotnet fullcalendarcore.dll
}
finally {
    Pop-Location
}
