# Build APK for Tailscale deployment
param(
    [Parameter(Mandatory=$true)]
    [string]$TailscaleIP,
    
    [Parameter(Mandatory=$false)]
    [string]$GoogleMapsApiKey = "YOUR_GOOGLE_MAPS_API_KEY_HERE"
)

Write-Host "🚀 Building Recollect APK for Tailscale deployment..." -ForegroundColor Green

# Update ConfigurationService with Tailscale IP
Write-Host "📝 Updating configuration for Tailscale IP: $TailscaleIP" -ForegroundColor Yellow

$configPath = "Recollect.Mobile/Services/ConfigurationService.cs"
if (Test-Path $configPath) {
    $content = Get-Content $configPath -Raw
    $content = $content -replace "YOUR_TAILSCALE_IP", $TailscaleIP
    Set-Content $configPath -Value $content -NoNewline
    Write-Host "✅ Configuration updated" -ForegroundColor Green
} else {
    Write-Host "❌ ConfigurationService.cs not found" -ForegroundColor Red
    exit 1
}

# Set environment variables
$env:GOOGLE_MAPS_API_KEY = $GoogleMapsApiKey

# Clean previous builds
Write-Host "🧹 Cleaning previous builds..." -ForegroundColor Yellow
if (Test-Path "Recollect.Mobile/bin") { Remove-Item "Recollect.Mobile/bin" -Recurse -Force }
if (Test-Path "Recollect.Mobile/obj") { Remove-Item "Recollect.Mobile/obj" -Recurse -Force }

# Build the project
Write-Host "🔨 Building mobile app..." -ForegroundColor Yellow
try {
    dotnet build Recollect.Mobile/Recollect.Mobile.csproj -c Release -f net9.0-android --no-restore
    if ($LASTEXITCODE -ne 0) {
        throw "Build failed with exit code $LASTEXITCODE"
    }
    Write-Host "✅ Build successful" -ForegroundColor Green
} catch {
    Write-Host "❌ Build failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Publish the APK
Write-Host "📱 Publishing APK..." -ForegroundColor Yellow
try {
    dotnet publish Recollect.Mobile/Recollect.Mobile.csproj -c Release -f net9.0-android -o publish/tailscale-apk
    if ($LASTEXITCODE -ne 0) {
        throw "Publish failed with exit code $LASTEXITCODE"
    }
    Write-Host "✅ APK published successfully" -ForegroundColor Green
} catch {
    Write-Host "❌ Publish failed: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
}

# Find the APK file
$apkPath = Get-ChildItem -Path "publish/tailscale-apk" -Filter "*.apk" -Recurse | Select-Object -First 1
if ($apkPath) {
    Write-Host "🎉 APK created successfully!" -ForegroundColor Green
    Write-Host "📱 APK Location: $($apkPath.FullName)" -ForegroundColor Cyan
    Write-Host "📏 APK Size: $([math]::Round($apkPath.Length / 1MB, 2)) MB" -ForegroundColor Cyan
} else {
    Write-Host "❌ APK file not found" -ForegroundColor Red
    exit 1
}

Write-Host ""
Write-Host "📋 Deployment Summary:" -ForegroundColor Cyan
Write-Host "  Tailscale IP: $TailscaleIP" -ForegroundColor White
Write-Host "  API URL: http://$TailscaleIP:7001" -ForegroundColor White
Write-Host "  APK Location: $($apkPath.FullName)" -ForegroundColor White
Write-Host ""
Write-Host "📱 Installation Instructions:" -ForegroundColor Cyan
Write-Host "1. Transfer the APK to your Android device" -ForegroundColor White
Write-Host "2. Enable 'Install from unknown sources' in Android settings" -ForegroundColor White
Write-Host "3. Install the APK" -ForegroundColor White
Write-Host "4. Install Tailscale app and join your network" -ForegroundColor White
Write-Host "5. Open Recollect app and go to Settings" -ForegroundColor White
Write-Host "6. Select 'tailscale' configuration" -ForegroundColor White
Write-Host "7. Test the connection" -ForegroundColor White
