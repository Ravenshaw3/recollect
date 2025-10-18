# Simple APK Build Script
Write-Host "Building Recollect Mobile APK..." -ForegroundColor Green

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
    Write-Host "Build successful!" -ForegroundColor Green
    
    # Publish
    Write-Host "Publishing APK..." -ForegroundColor Yellow
    dotnet publish -c Release -f net9.0-android -o ./publish
    
    # Create output directory
    $outputDir = "../publish/android-final"
    if (Test-Path $outputDir) {
        Remove-Item $outputDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $outputDir -Force
    
    # Copy APK files
    if (Test-Path "./publish/com.companyname.recollect.mobile.apk") {
        Copy-Item "./publish/com.companyname.recollect.mobile.apk" -Destination "$outputDir/recollect-mobile.apk"
        Write-Host "APK copied successfully!" -ForegroundColor Green
    }
    
    if (Test-Path "./publish/com.companyname.recollect.mobile-Signed.apk") {
        Copy-Item "./publish/com.companyname.recollect.mobile-Signed.apk" -Destination "$outputDir/recollect-mobile-signed.apk"
        Write-Host "Signed APK copied successfully!" -ForegroundColor Green
    }
    
    Write-Host "APK build complete!" -ForegroundColor Green
    Write-Host "Location: $outputDir" -ForegroundColor Cyan
} else {
    Write-Host "Build failed!" -ForegroundColor Red
}

# Return to root
Set-Location ..

Write-Host "Done!" -ForegroundColor Green
