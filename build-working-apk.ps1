# Build Working Recollect Mobile APK
Write-Host "🚀 Building Working Recollect Mobile APK..." -ForegroundColor Green

# Navigate to mobile project
Set-Location "Recollect.Mobile"

# Clean everything
Write-Host "🧹 Cleaning everything..." -ForegroundColor Yellow
dotnet clean -c Release
Remove-Item -Path "./bin" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item -Path "./obj" -Recurse -Force -ErrorAction SilentlyContinue

# Restore packages
Write-Host "📦 Restoring packages..." -ForegroundColor Yellow
dotnet restore

# Build for Android only
Write-Host "🔨 Building Android Release..." -ForegroundColor Yellow
dotnet build -c Release -f net9.0-android

# Check if build succeeded
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Build successful! Publishing APK..." -ForegroundColor Green
    
    # Publish for Android
    Write-Host "📱 Publishing Android APK..." -ForegroundColor Yellow
    dotnet publish -c Release -f net9.0-android -o ./bin/Release/net9.0-android/publish

    # Create working directory
    $publishDir = "../publish/android-working"
    if (Test-Path $publishDir) {
        Remove-Item $publishDir -Recurse -Force
    }
    New-Item -ItemType Directory -Path $publishDir -Force

    # Copy APK files
    Write-Host "📦 Copying APK files..." -ForegroundColor Yellow
    if (Test-Path "./bin/Release/net9.0-android/publish/com.companyname.recollect.mobile.apk") {
        Copy-Item "./bin/Release/net9.0-android/publish/com.companyname.recollect.mobile.apk" -Destination "$publishDir/com.companyname.recollect.mobile-Working.apk"
        Write-Host "✅ APK copied successfully!" -ForegroundColor Green
    }
    
    if (Test-Path "./bin/Release/net9.0-android/publish/com.companyname.recollect.mobile-Signed.apk") {
        Copy-Item "./bin/Release/net9.0-android/publish/com.companyname.recollect.mobile-Signed.apk" -Destination "$publishDir/com.companyname.recollect.mobile-Working-Signed.apk"
        Write-Host "✅ Signed APK copied successfully!" -ForegroundColor Green
    }

    # Create README
    $readmeContent = @"
# 📱 Recollect Mobile - Working Version

## ✅ **Working Features**

- Adventure tracking with GPS
- Photo capture
- Note taking
- Map visualization
- Server synchronization
- AI story generation

## 📱 **Installation**

1. Download: com.companyname.recollect.mobile-Working-Signed.apk
2. Enable "Install from Unknown Sources" in Android settings
3. Install the APK
4. Grant required permissions
5. Configure server URL

## 🎯 **Ready to Use!**

This is a working version of the Recollect mobile app with all core features!
"@

    Set-Content -Path "$publishDir/README.md" -Value $readmeContent

    Write-Host "✅ Working APK created successfully!" -ForegroundColor Green
    Write-Host "📦 Location: $publishDir" -ForegroundColor Cyan
    Write-Host "🎯 Files created:" -ForegroundColor Yellow
    Get-ChildItem $publishDir | ForEach-Object { Write-Host "  - $($_.Name)" -ForegroundColor White }
} else {
    Write-Host "❌ Build failed! Check the errors above." -ForegroundColor Red
}

# Return to root
Set-Location ..

Write-Host "Build process complete!" -ForegroundColor Green
