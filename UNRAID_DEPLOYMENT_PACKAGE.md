# Recollect API - unRAID Deployment Package

## 📦 **What's Included**

### **Docker Templates**
- ✅ `unraid-docker-template.xml` - API container template
- ✅ `unraid-docker-template-db.xml` - Database container template
- ✅ Icons included (using official .NET and PostgreSQL icons)

### **Build Scripts**
- ✅ `build-docker.sh` - Docker build script
- ✅ `docker-compose.yaml` - Alternative deployment method

### **Configuration Files**
- ✅ `Dockerfile.dockerfile` - Updated for unRAID deployment
- ✅ All admin dashboard files included

## 🚀 **Deployment Steps**

### **Step 1: Prepare unRAID Server**
```bash
# Create directories on unRAID
mkdir -p /mnt/user/appdata/recollect
mkdir -p /mnt/user/appdata/recollect-db
```

### **Step 2: Upload Project**
```bash
# Copy entire project to unRAID server
# Place in: /mnt/user/appdata/recollect/
```

### **Step 3: Build Docker Image**
```bash
# On unRAID server
cd /mnt/user/appdata/recollect/
chmod +x build-docker.sh
./build-docker.sh
```

### **Step 4: Import Templates**
1. Go to unRAID Docker tab
2. Click "Add Container"
3. Select "Add from template"
4. Import `unraid-docker-template-db.xml` (Database first)
5. Import `unraid-docker-template.xml` (API second)

### **Step 5: Configure Containers**
- **Database Container**: Use default settings
- **API Container**: Ensure database connection string points to `recollect-db`

## 🎯 **Template Features**

### **API Container Template**
- **Name**: Recollect-API
- **Icon**: .NET logo
- **Ports**: 7001 (HTTP), 7002 (HTTPS)
- **WebUI**: http://[IP]:7001/admin
- **Category**: Network:WebServer
- **Auto-restart**: Enabled

### **Database Container Template**
- **Name**: Recollect-Database
- **Icon**: PostgreSQL logo
- **Port**: 5432
- **Category**: Database
- **Auto-restart**: Enabled

## 🔧 **Configuration Options**

### **API Container Settings**
- **Environment**: Production/Development
- **Database Connection**: Configurable
- **Data Directory**: `/mnt/user/appdata/recollect`
- **Ports**: Customizable

### **Database Container Settings**
- **Database Name**: Recollect
- **Username**: postgres
- **Password**: Configurable
- **Data Directory**: `/mnt/user/appdata/recollect-db`

## 📱 **Access Points**

### **Admin Dashboard**
- **URL**: `http://your-unraid-ip:7001/admin`
- **Features**: Full management interface

### **API Endpoints**
- **Base**: `http://your-unraid-ip:7001/api/`
- **Adventures**: `http://your-unraid-ip:7001/api/adventures`
- **Admin**: `http://your-unraid-ip:7001/api/admin/`

## ✅ **Templates Status**

### **✅ Built and Ready**
- [x] API container template
- [x] Database container template
- [x] Icons included
- [x] Configuration options
- [x] Port mappings
- [x] Volume mounts
- [x] Environment variables

### **✅ Icons Available**
- **API**: .NET logo (official Microsoft icon)
- **Database**: PostgreSQL logo (official PostgreSQL icon)

## 🚀 **Quick Start**

1. **Upload project to unRAID**
2. **Run build script**: `./build-docker.sh`
3. **Import templates** in unRAID Docker interface
4. **Start containers** in order: Database first, then API
5. **Access admin dashboard**: `http://your-unraid-ip:7001/admin`

## 📋 **Requirements**

- unRAID 6.8+ with Docker support
- At least 2GB RAM for containers
- Ports 7001, 7002, 5432 available
- Basic unRAID Docker knowledge

## 🎉 **Ready to Deploy!**

The templates are fully built and ready for unRAID deployment. Just follow the steps above and you'll have your Recollect API with admin dashboard running on your unRAID server!
