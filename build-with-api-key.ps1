# Build Recollect Mobile APK with Google Maps API Key
param(
    [string]$GoogleMapsApiKey = ""
)

if ([string]::IsNullOrEmpty($GoogleMapsApiKey)) {
    Write-Host "Please provide your Google Maps API Key:" -ForegroundColor Yellow
    Write-Host "Usage: .\build-with-api-key.ps1 -GoogleMapsApiKey 'YOUR_API_KEY_HERE'" -ForegroundColor Cyan
    Write-Host "Or set environment variable: `$env:GOOGLE_MAPS_API_KEY='YOUR_API_KEY_HERE'" -ForegroundColor Cyan
    exit 1
}

# Set environment variable
$env:GOOGLE_MAPS_API_KEY = $GoogleMapsApiKey

Write-Host "Building Recollect Mobile APK with Google Maps API Key..." -ForegroundColor Green
Write-Host "API Key: $($GoogleMapsApiKey.Substring(0, [Math]::Min(10, $GoogleMapsApiKey.Length)))..." -ForegroundColor Yellow

# Navigate to mobile project
Set-Location "Recollect.Mobile"

# Clean and restore
Write-Host "Cleaning and restoring..." -ForegroundColor Yellow
dotnet clean -c Release
dotnet restore

# Build for Android
Write-Host "Building Android..." -ForegroundColor Yellow
dotnet build -c Release -f net9.0-android

# Check if successful
if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful! Publishing APK..." -ForegroundColor Green
    
    # Publish
    Write-Host "Publishing APK..." -ForegroundColor Yellow
    dotnet publish -c Release -f net9.0-android -o ./publish
    
    # Create output directory
    $outputDir = "../publish/android-with-maps"
    if (Test-Path $outputDir) {
        Remove-Item $outputDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $outputDir -Force
    
    # Copy APK files
    Write-Host "Copying APK files..." -ForegroundColor Yellow
    if (Test-Path "./publish/com.companyname.recollect.mobile.apk") {
        Copy-Item "./publish/com.companyname.recollect.mobile.apk" -Destination "$outputDir/recollect-mobile-with-maps.apk"
        Write-Host "APK copied successfully!" -ForegroundColor Green
    }
    
    if (Test-Path "./publish/com.companyname.recollect.mobile-Signed.apk") {
        Copy-Item "./publish/com.companyname.recollect.mobile-Signed.apk" -Destination "$outputDir/recollect-mobile-with-maps-signed.apk"
        Write-Host "Signed APK copied successfully!" -ForegroundColor Green
    }
    
    # Create README
    $readmeContent = @"
# Recollect Mobile - With Google Maps

## Features
- Adventure tracking with GPS
- Google Maps integration
- Photo capture
- Audio upload (voice notes)
- Note taking
- Server synchronization

## Installation
1. Download: recollect-mobile-with-maps-signed.apk
2. Enable 'Install from Unknown Sources' in Android settings
3. Install the APK
4. Grant required permissions
5. Configure server URL

## Google Maps API Key
This build includes Google Maps integration. Make sure your API key has the following restrictions:
- Android apps restriction
- Maps SDK for Android enabled
- Geocoding API enabled (if using address lookup)

## Ready to Use!
This version includes Google Maps and audio upload features.
"@

    Set-Content -Path "$outputDir/README.md" -Value $readmeContent
    
    Write-Host "APK with Google Maps created successfully!" -ForegroundColor Green
    Write-Host "Location: $outputDir" -ForegroundColor Cyan
    Write-Host "Files created:" -ForegroundColor Yellow
    Get-ChildItem $outputDir | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor White }
} else {
    Write-Host "Build failed! Check the errors above." -ForegroundColor Red
}

# Return to root
Set-Location ..

Write-Host "Build process complete!" -ForegroundColor Green
