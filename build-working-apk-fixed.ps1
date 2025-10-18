# Build Working Recollect Mobile APK - Fixed Version
Write-Host "Building Working Recollect Mobile APK..." -ForegroundColor Green

# Navigate to mobile project
Set-Location "Recollect.Mobile"

# Clean everything
Write-Host "Cleaning everything..." -ForegroundColor Yellow
dotnet clean -c Release
Remove-Item -Path "./bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "./obj" -Recurse -Force -ErrorAction SilentlyContinue

# Restore packages
Write-Host "Restoring packages..." -ForegroundColor Yellow
dotnet restore

# Build for Android only
Write-Host "Building Android Release..." -ForegroundColor Yellow
dotnet build -c Release -f net9.0-android

# Check if build succeeded
if ($LASTEXITCODE -eq 0) {
    Write-Host "Build successful! Publishing APK..." -ForegroundColor Green
    
    # Publish for Android
    Write-Host "Publishing Android APK..." -ForegroundColor Yellow
    dotnet publish -c Release -f net9.0-android -o ./publish

    # Create output directory
    $outputDir = "../publish/android-working"
    if (Test-Path $outputDir) {
        Remove-Item $outputDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $outputDir -Force
    
    # Copy APK files
    Write-Host "Copying APK files..." -ForegroundColor Yellow
    if (Test-Path "./publish/com.companyname.recollect.mobile.apk") {
        Copy-Item "./publish/com.companyname.recollect.mobile.apk" -Destination "$outputDir/recollect-mobile.apk"
        Write-Host "APK copied successfully!" -ForegroundColor Green
    }
    
    if (Test-Path "./publish/com.companyname.recollect.mobile-Signed.apk") {
        Copy-Item "./publish/com.companyname.recollect.mobile-Signed.apk" -Destination "$outputDir/recollect-mobile-signed.apk"
        Write-Host "Signed APK copied successfully!" -ForegroundColor Green
    }
    
    # Create README
    $readmeContent = @"
# Recollect Mobile - Working APK

## Features
- Adventure tracking with GPS
- Photo capture
- Note taking
- Map visualization
- Server synchronization

## Installation
1. Download: recollect-mobile-signed.apk
2. Enable 'Install from Unknown Sources' in Android settings
3. Install the APK
4. Grant required permissions
5. Configure server URL

## Ready to Use!
This is a working version of the Recollect mobile app.
"@

    Set-Content -Path "$outputDir/README.md" -Value $readmeContent
    
    Write-Host "Working APK created successfully!" -ForegroundColor Green
    Write-Host "Location: $outputDir" -ForegroundColor Cyan
    Write-Host "Files created:" -ForegroundColor Yellow
    Get-ChildItem $outputDir | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor White }
} else {
    Write-Host "Build failed! Check the errors above." -ForegroundColor Red
}

# Return to root
Set-Location ..

Write-Host "Build process complete!" -ForegroundColor Green
