# Tailscale Deployment Script for Recollect (PowerShell)
# This script helps configure the mobile app for Tailscale access

param(
    [Parameter(Mandatory=$true)]
    [string]$TailscaleIP,
    
    [Parameter(Mandatory=$false)]
    [string]$GoogleMapsApiKey = "YOUR_GOOGLE_MAPS_API_KEY_HERE"
)

Write-Host "🚀 Configuring Recollect for Tailscale access..." -ForegroundColor Green

# Update ConfigurationService.cs with Tailscale IP
$configServicePath = "Recollect.Mobile/Services/ConfigurationService.cs"
if (Test-Path $configServicePath) {
    Write-Host "📝 Updating ConfigurationService.cs with Tailscale IP: $TailscaleIP" -ForegroundColor Yellow
    
    $content = Get-Content $configServicePath -Raw
    $content = $content -replace "YOUR_TAILSCALE_IP", $TailscaleIP
    Set-Content $configServicePath -Value $content -NoNewline
    
    Write-Host "✅ ConfigurationService.cs updated" -ForegroundColor Green
} else {
    Write-Host "❌ ConfigurationService.cs not found" -ForegroundColor Red
    exit 1
}

# Build the mobile app
Write-Host "🔨 Building mobile app..." -ForegroundColor Yellow
try {
    dotnet build Recollect.Mobile/Recollect.Mobile.csproj -c Release -f net9.0-android
    Write-Host "✅ Mobile app built successfully" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed to build mobile app: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Build APK with API key
Write-Host "📱 Building APK with Google Maps API key..." -ForegroundColor Yellow
try {
    $env:GOOGLE_MAPS_API_KEY = $GoogleMapsApiKey
    dotnet publish Recollect.Mobile/Recollect.Mobile.csproj -c Release -f net9.0-android -o publish/android
    Write-Host "✅ APK built successfully" -ForegroundColor Green
} catch {
    Write-Host "❌ Failed to build APK: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

Write-Host "🎉 Tailscale configuration completed!" -ForegroundColor Green
Write-Host ""
Write-Host "📋 Summary:" -ForegroundColor Cyan
Write-Host "  Tailscale IP: $TailscaleIP" -ForegroundColor White
Write-Host "  API URL: http://$TailscaleIP:7001" -ForegroundColor White
Write-Host "  Admin Interface: http://$TailscaleIP:7001/admin" -ForegroundColor White
Write-Host ""
Write-Host "📱 Next steps:" -ForegroundColor Cyan
Write-Host "1. Install the APK on your Android device" -ForegroundColor White
Write-Host "2. Install Tailscale on your mobile device" -ForegroundColor White
Write-Host "3. Join the same Tailscale network as your unRAID server" -ForegroundColor White
Write-Host "4. Open the Recollect app and go to Settings" -ForegroundColor White
Write-Host "5. Select 'tailscale' configuration and test connection" -ForegroundColor White
Write-Host ""
Write-Host "🌐 Access from other devices:" -ForegroundColor Cyan
Write-Host "  Admin Interface: http://$TailscaleIP:7001/admin" -ForegroundColor White
Write-Host "  API Endpoint: http://$TailscaleIP:7001/api/adventures" -ForegroundColor White
