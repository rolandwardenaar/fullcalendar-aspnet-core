# Quick Start Script - Start de applicatie zonder publish
# Voor development/testing op poort 8086

Write-Host "🚀 Starting FullCalendar on port 8086..." -ForegroundColor Cyan
Write-Host ""

# Set environment variables
$env:ASPNETCORE_URLS = "http://localhost:8086"
$env:ASPNETCORE_ENVIRONMENT = "Production"

Write-Host "📍 Server URL: http://localhost:8086" -ForegroundColor Green
Write-Host "🌍 Environment: Production" -ForegroundColor Green
Write-Host ""
Write-Host "Press Ctrl+C to stop" -ForegroundColor Gray
Write-Host ""

# Start zonder publish (sneller voor testing)
dotnet run --no-build
