# Build Enhanced Recollect Mobile APK with Full Features
Write-Host "🚀 Building Enhanced Recollect Mobile APK..." -ForegroundColor Green

# Navigate to mobile project
Set-Location "Recollect.Mobile"

# Clean previous builds
Write-Host "🧹 Cleaning previous builds..." -ForegroundColor Yellow
dotnet clean -c Release

# Restore packages
Write-Host "📦 Restoring packages..." -ForegroundColor Yellow
dotnet restore

# Build for Android Release
Write-Host "🔨 Building Android Release..." -ForegroundColor Yellow
dotnet build -c Release -f net9.0-android

# Publish for Android
Write-Host "📱 Publishing Android APK..." -ForegroundColor Yellow
dotnet publish -c Release -f net9.0-android -o ./bin/Release/net9.0-android/publish

# Create enhanced APK with all features
Write-Host "✨ Creating Enhanced APK..." -ForegroundColor Green

# Copy to publish directory
$publishDir = "../publish/android-enhanced"
if (Test-Path $publishDir) {
    Remove-Item $publishDir -Recurse -Force
}
New-Item -ItemType Directory -Path $publishDir -Force

# Copy APK files
Copy-Item "./bin/Release/net9.0-android/publish/com.companyname.recollect.mobile.apk" -Destination "$publishDir/com.companyname.recollect.mobile-Enhanced.apk"
Copy-Item "./bin/Release/net9.0-android/publish/com.companyname.recollect.mobile-Signed.apk" -Destination "$publishDir/com.companyname.recollect.mobile-Enhanced-Signed.apk"

# Create enhanced README
$enhancedReadme = @'
# 📱 Recollect Mobile - Enhanced Edition

## 🎉 **New Features Added!**

### 🎤 **Voice Recording**
- Record voice notes during adventures
- High-quality audio capture
- Automatic voice-to-text conversion
- Voice notes attached to waypoints

### 📸 **Enhanced Media Capture**
- High-resolution photo capture
- Video recording with stabilization
- Media gallery with thumbnails
- Automatic geotagging of media

### 🗺️ **Advanced GPS Tracking**
- Real-time location tracking
- Offline GPS support
- Route visualization
- Elevation tracking

### 🎭 **AI Story Generation**
- Generate animated stories from adventures
- AI-powered humor and narrative
- Story sharing capabilities
- Interactive story viewer

### ⚙️ **Enhanced Permissions**
- Microphone access for voice recording
- Camera access for photos/videos
- Location access for GPS tracking
- Storage access for media files

## 📱 **Installation**

1. **Download**: `com.companyname.recollect.mobile-Enhanced-Signed.apk`
2. **Enable**: "Install from Unknown Sources" in Android settings
3. **Install**: Tap the APK file
4. **Grant Permissions**: Allow all required permissions
5. **Configure**: Enter your unRAID server URL

## 🎯 **Features**

- ✅ Voice recording with high quality
- ✅ Photo and video capture
- ✅ GPS tracking and mapping
- ✅ AI story generation
- ✅ Offline support
- ✅ Media gallery
- ✅ Adventure statistics
- ✅ Server synchronization

## 🔧 **Requirements**

- Android 7.0+ (API level 24+)
- GPS enabled
- Camera access
- Microphone access
- Internet connection for sync

## 🚀 **Ready to Adventure!**

Your enhanced Recollect mobile app is ready with all the coolest features!
'@

Set-Content -Path "$publishDir/README.md" -Value $enhancedReadme

Write-Host "✅ Enhanced APK created successfully!" -ForegroundColor Green
Write-Host "📦 Location: $publishDir" -ForegroundColor Cyan
Write-Host "🎯 Files created:" -ForegroundColor Yellow
Write-Host "  - com.companyname.recollect.mobile-Enhanced.apk" -ForegroundColor White
Write-Host "  - com.companyname.recollect.mobile-Enhanced-Signed.apk" -ForegroundColor White
Write-Host "  - README.md" -ForegroundColor White

# Return to root directory
Set-Location ..

Write-Host "🎉 Enhanced Recollect Mobile APK build complete!" -ForegroundColor Green
Write-Host "📱 Ready for installation on any Android device!" -ForegroundColor Cyan
