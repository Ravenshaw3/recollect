# Build Enhanced Recollect Mobile APK
Write-Host "🚀 Building Enhanced Recollect Mobile APK..." -ForegroundColor Green

# Navigate to mobile project
Set-Location "Recollect.Mobile"

# Clean and restore
Write-Host "🧹 Cleaning and restoring..." -ForegroundColor Yellow
dotnet clean -c Release
dotnet restore

# Build for Android
Write-Host "🔨 Building Android Release..." -ForegroundColor Yellow
dotnet build -c Release -f net9.0-android

# Publish for Android
Write-Host "📱 Publishing Android APK..." -ForegroundColor Yellow
dotnet publish -c Release -f net9.0-android -o ./bin/Release/net9.0-android/publish

# Create enhanced directory
$publishDir = "../publish/android-enhanced"
if (Test-Path $publishDir) {
    Remove-Item $publishDir -Recurse -Force
}
New-Item -ItemType Directory -Path $publishDir -Force

# Copy APK files
Write-Host "📦 Copying APK files..." -ForegroundColor Yellow
Copy-Item "./bin/Release/net9.0-android/publish/com.companyname.recollect.mobile.apk" -Destination "$publishDir/com.companyname.recollect.mobile-Enhanced.apk" -ErrorAction SilentlyContinue
Copy-Item "./bin/Release/net9.0-android/publish/com.companyname.recollect.mobile-Signed.apk" -Destination "$publishDir/com.companyname.recollect.mobile-Enhanced-Signed.apk" -ErrorAction SilentlyContinue

# Create simple README
$readmeContent = "Enhanced Recollect Mobile APK with Voice Recording and Cool Features"
Set-Content -Path "$publishDir/README.txt" -Value $readmeContent

Write-Host "✅ Enhanced APK created successfully!" -ForegroundColor Green
Write-Host "📦 Location: $publishDir" -ForegroundColor Cyan

# Return to root
Set-Location ..

Write-Host "🎉 Enhanced Recollect Mobile APK build complete!" -ForegroundColor Green
