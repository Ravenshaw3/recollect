# 📱 Mobile App Troubleshooting Guide

## 🚨 **Common Issues & Solutions**

### **Issue 1: App Won't Install**

#### **Problem**: "Installation blocked" or "Unknown sources"
#### **Solution**:
1. **Go to Android Settings**
2. **Security & Privacy** → **Install unknown apps**
3. **Enable for your file manager/browser**
4. **Try installing again**

### **Issue 2: App Crashes on Startup**

#### **Problem**: App closes immediately after opening
#### **Solution**:
1. **Check Android version** (requires 7.0+)
2. **Clear app data**: Settings → Apps → Recollect → Storage → Clear Data
3. **Restart device**
4. **Reinstall the app**

### **Issue 3: Can't Connect to Server**

#### **Problem**: "Connection failed" or "Server not found"
#### **Solution**:
1. **Verify server URL**: `http://YOUR-UNRAID-IP:7001`
2. **Check network connectivity**
3. **Ensure unRAID server is running**
4. **Test in browser**: `http://YOUR-UNRAID-IP:7001/admin`

### **Issue 4: GPS Not Working**

#### **Problem**: "Location not found" or GPS not tracking
#### **Solution**:
1. **Enable location services**: Settings → Location → On
2. **Grant location permission** to Recollect app
3. **Set location accuracy** to "High"
4. **Test GPS** with other apps first

### **Issue 5: Camera Not Working**

#### **Problem**: Camera won't open or crashes
#### **Solution**:
1. **Grant camera permission** to Recollect app
2. **Check if camera is used by another app**
3. **Restart the app**
4. **Test camera** with other apps

## 🔧 **Step-by-Step Debugging**

### **Step 1: Check Prerequisites**
- [ ] Android 7.0+ (API level 24+)
- [ ] Internet connection
- [ ] unRAID server running
- [ ] Server accessible from mobile network

### **Step 2: Verify Installation**
```bash
# Check if app is installed
adb shell pm list packages | grep recollect

# Check app permissions
adb shell dumpsys package com.companyname.recollect.mobile
```

### **Step 3: Test Server Connection**
```bash
# Test from mobile device
curl http://YOUR-UNRAID-IP:7001/admin

# Check server logs
docker logs recollect-api
```

### **Step 4: Check App Logs**
```bash
# View app logs
adb logcat | grep recollect

# Check for errors
adb logcat | grep -i error
```

## 🛠️ **Advanced Troubleshooting**

### **Network Issues**
1. **Check firewall** on unRAID server
2. **Verify port 7001** is accessible
3. **Test with different network** (WiFi vs mobile data)
4. **Check DNS resolution**

### **Permission Issues**
1. **Grant all required permissions**:
   - Location (GPS)
   - Camera
   - Storage
   - Internet
2. **Check app permissions** in Android settings
3. **Reset permissions** and grant again

### **Performance Issues**
1. **Close other apps** to free memory
2. **Restart device** to clear cache
3. **Check available storage** space
4. **Update Android** if possible

## 📊 **Testing Checklist**

### **Before Installation**
- [ ] Android 7.0+ ✓
- [ ] Internet connection ✓
- [ ] unRAID server running ✓
- [ ] Server accessible ✓

### **After Installation**
- [ ] App opens without crashing ✓
- [ ] Can connect to server ✓
- [ ] GPS tracking works ✓
- [ ] Camera works ✓
- [ ] Can create adventures ✓

### **Full Functionality Test**
- [ ] Start adventure tracking ✓
- [ ] Add waypoints ✓
- [ ] Take photos/videos ✓
- [ ] Add notes ✓
- [ ] Stop adventure ✓
- [ ] Sync with server ✓

## 🆘 **Still Not Working?**

### **Alternative Solutions**
1. **Try unsigned APK**: `com.companyname.recollect.mobile.apk`
2. **Install via ADB**: `adb install com.companyname.recollect.mobile-Signed.apk`
3. **Check device compatibility**
4. **Contact support** with error logs

### **Error Reporting**
When reporting issues, include:
- **Android version**
- **Device model**
- **Error messages**
- **Steps to reproduce**
- **Log files** (if available)

## 🎯 **Success Indicators**

### **App Working Correctly**
- ✅ Opens without crashing
- ✅ Connects to server
- ✅ GPS tracking active
- ✅ Camera functional
- ✅ Can create adventures
- ✅ Data syncs with server

---

**Follow this guide step by step to resolve mobile app issues! 📱🔧✨**
