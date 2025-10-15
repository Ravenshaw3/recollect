# 🚀 unRAID Deployment Guide - Recollect Adventure Tracking

## 📋 **Prerequisites**
- unRAID server with Docker support
- Internet connection for downloading containers
- Access to unRAID web interface

## 🎯 **Step-by-Step Deployment**

### **Step 1: Download the Docker Templates**

1. **Download API Template**:
   - Go to: [https://raw.githubusercontent.com/Ravenshaw3/recollect/main/unraid-docker-template.xml](https://raw.githubusercontent.com/Ravenshaw3/recollect/main/unraid-docker-template.xml)
   - Right-click → "Save As" → Save as `recollect-api-template.xml`

2. **Download Database Template**:
   - Go to: [https://raw.githubusercontent.com/Ravenshaw3/recollect/main/unraid-docker-template-db.xml](https://raw.githubusercontent.com/Ravenshaw3/recollect/main/unraid-docker-template-db.xml)
   - Right-click → "Save As" → Save as `recollect-db-template.xml`

### **Step 2: Install Templates in unRAID**

#### **Option A: Manual Template Installation**
1. **Access unRAID Web Interface**
2. **Go to Docker Tab**
3. **Click "Add Container"**
4. **Click "Add from template"**
5. **Upload the XML files**:
   - Upload `recollect-api-template.xml`
   - Upload `recollect-db-template.xml`

#### **Option B: Direct GitHub Import**
1. **In unRAID Docker tab**
2. **Click "Add Container"**
3. **Click "Add from template"**
4. **Use these URLs**:
   - API: `https://raw.githubusercontent.com/Ravenshaw3/recollect/main/unraid-docker-template.xml`
   - DB: `https://raw.githubusercontent.com/Ravenshaw3/recollect/main/unraid-docker-template-db.xml`

### **Step 3: Configure and Deploy**

#### **Deploy Database First (Important!)**
1. **Select "Recollect-Database" template**
2. **Configure Settings**:
   - **Port**: 5432 (default)
   - **Data Directory**: `/mnt/user/appdata/recollect-db`
   - **Database Name**: `Recollect`
   - **Username**: `postgres`
   - **Password**: `secret` (change this!)
3. **Click "Apply"**
4. **Wait for database to start** (check logs)

#### **Deploy API Container**
1. **Select "Recollect-API" template**
2. **Configure Settings**:
   - **Port 7001**: 7001 (HTTP)
   - **Port 7002**: 7002 (HTTPS - optional)
   - **Data Directory**: `/mnt/user/appdata/recollect`
   - **Database Connection**: `Host=recollect-database;Database=Recollect;Username=postgres;Password=secret`
3. **Click "Apply"**
4. **Wait for API to start**

### **Step 4: Verify Deployment**

#### **Check Container Status**
1. **Go to Docker tab**
2. **Verify both containers are running**:
   - ✅ `Recollect-Database` (PostgreSQL)
   - ✅ `Recollect-API` (API Server)

#### **Test Access**
1. **Open browser**
2. **Go to**: `http://YOUR-UNRAID-IP:7001/admin`
3. **You should see the Recollect Admin Dashboard**

### **Step 5: Configure Network (if needed)**

#### **If containers can't communicate**:
1. **Go to Docker tab**
2. **Click on "Recollect-API" container**
3. **Click "Edit"**
4. **In "Extra Parameters" add**:
   ```
   --link recollect-database:recollect-db
   ```
5. **Click "Apply"**

## 🔧 **Configuration Options**

### **Port Configuration**
- **API HTTP**: 7001 (required)
- **API HTTPS**: 7002 (optional)
- **Database**: 5432 (internal)

### **Data Directories**
- **API Data**: `/mnt/user/appdata/recollect`
- **Database**: `/mnt/user/appdata/recollect-db`

### **Environment Variables**
- **ASPNETCORE_ENVIRONMENT**: `Production`
- **Database Connection**: Auto-configured
- **Restart Policy**: `unless-stopped`

## 🎭 **Using the Story Generator**

### **Access the Admin Dashboard**
1. **Open**: `http://YOUR-UNRAID-IP:7001/admin`
2. **Click "Story Generator" tab**
3. **Create your first adventure story!**

### **Story Generator Features**
- **Random Story**: Generate stories from any adventure
- **Animated Stories**: Visual effects and animations
- **AI Humor**: Transform GPS data into comedy gold
- **Statistics**: Adventure metrics and performance

## 🛠️ **Troubleshooting**

### **Container Won't Start**
1. **Check logs**: Click on container → "Logs"
2. **Verify ports**: Make sure 7001 and 5432 are available
3. **Check data directories**: Ensure paths exist

### **Database Connection Issues**
1. **Verify database is running first**
2. **Check connection string in API container**
3. **Ensure containers are on same network**

### **Admin Dashboard Not Loading**
1. **Check API container logs**
2. **Verify port 7001 is accessible**
3. **Try**: `http://YOUR-UNRAID-IP:7001/admin/index.html`

## 📱 **Mobile App Integration**

### **Connect Mobile App**
1. **Install the Recollect mobile app**
2. **Configure API endpoint**: `http://YOUR-UNRAID-IP:7001`
3. **Start tracking adventures!**

## 🔒 **Security Considerations**

### **Change Default Passwords**
- **Database Password**: Change from `secret` to something secure
- **API Access**: Consider adding authentication

### **Network Security**
- **Firewall**: Only expose necessary ports
- **VPN**: Consider VPN access for remote management

## 📊 **Monitoring**

### **Container Health**
- **Check Docker tab regularly**
- **Monitor resource usage**
- **Review logs for errors**

### **Performance**
- **Database**: Monitor disk usage in `/mnt/user/appdata/recollect-db`
- **API**: Check memory usage and response times

## 🎉 **Success!**

Once deployed, you'll have:
- ✅ **Adventure Tracking API** running on unRAID
- ✅ **Admin Dashboard** for managing adventures
- ✅ **AI Story Generator** for creating animated stories
- ✅ **PostgreSQL Database** for data storage
- ✅ **Mobile App Support** for live tracking

## 📞 **Support**

- **GitHub Issues**: [https://github.com/Ravenshaw3/recollect/issues](https://github.com/Ravenshaw3/recollect/issues)
- **Documentation**: [https://github.com/Ravenshaw3/recollect](https://github.com/Ravenshaw3/recollect)
- **unRAID Community**: Check unRAID forums for Docker help

---

**Your Recollect Adventure Tracking System is now running on unRAID! 🚀🗺️🎭**
