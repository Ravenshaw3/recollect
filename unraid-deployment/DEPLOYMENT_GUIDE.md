# 🚀 unRAID Docker Compose Deployment Guide

## 📋 **Prerequisites**
- unRAID server with Docker support
- SSH access to unRAID server
- Internet connection

## 🎯 **Step-by-Step Deployment**

### **Step 1: Prepare Files on Your Local Machine**

1. **Copy the deployment files to your unRAID server**:
   - `docker-compose-unraid.yaml`
   - `build-api.sh` (Linux/Mac) or `build-api.ps1` (Windows)
   - `README.md`

### **Step 2: Build the API Image**

#### **Option A: Build on unRAID Server**
```bash
# SSH into your unRAID server
ssh root@YOUR-UNRAID-IP

# Navigate to deployment directory
cd /mnt/user/appdata/recollect-deployment

# Build the API image
./build-api.sh
```

#### **Option B: Build Locally and Transfer**
```bash
# On your local machine
./build-api.ps1  # Windows PowerShell
# or
./build-api.sh  # Linux/Mac

# Save the image to a file
docker save recollect-api:latest -o recollect-api.tar

# Transfer to unRAID server
scp recollect-api.tar root@YOUR-UNRAID-IP:/mnt/user/appdata/

# On unRAID server, load the image
docker load -i recollect-api.tar
```

### **Step 3: Deploy with Docker Compose**

```bash
# SSH into your unRAID server
ssh root@YOUR-UNRAID-IP

# Navigate to deployment directory
cd /mnt/user/appdata/recollect-deployment

# Start the services
docker-compose -f docker-compose-unraid.yaml up -d

# Check status
docker-compose -f docker-compose-unraid.yaml ps
```

### **Step 4: Verify Deployment**

1. **Check container status**:
   ```bash
   docker ps
   ```

2. **Check logs**:
   ```bash
   docker-compose -f docker-compose-unraid.yaml logs
   ```

3. **Access the admin dashboard**:
   - Open browser: `http://YOUR-UNRAID-IP:7001/admin`

## 🔧 **Configuration Options**

### **Ports**
- **API**: 7001 (HTTP)
- **Database**: 5432 (internal only)

### **Data Directories**
- **API Data**: `/mnt/user/appdata/recollect`
- **Database**: `/mnt/user/appdata/recollect-db`

### **Environment Variables**
- **ASPNETCORE_ENVIRONMENT**: `Production`
- **Database Connection**: Auto-configured

## 🛠️ **Management Commands**

### **Start Services**
```bash
docker-compose -f docker-compose-unraid.yaml up -d
```

### **Stop Services**
```bash
docker-compose -f docker-compose-unraid.yaml down
```

### **View Logs**
```bash
docker-compose -f docker-compose-unraid.yaml logs -f
```

### **Restart Services**
```bash
docker-compose -f docker-compose-unraid.yaml restart
```

### **Update Services**
```bash
docker-compose -f docker-compose-unraid.yaml pull
docker-compose -f docker-compose-unraid.yaml up -d
```

## 🎭 **Using the System**

### **Admin Dashboard**
- **URL**: `http://YOUR-UNRAID-IP:7001/admin`
- **Features**: Adventure management, story generator, statistics

### **Story Generator**
1. **Access admin dashboard**
2. **Click "Story Generator" tab**
3. **Create animated stories from your adventures!**

### **Mobile App**
- **Configure API endpoint**: `http://YOUR-UNRAID-IP:7001`
- **Start tracking adventures**

## 🐛 **Troubleshooting**

### **Containers Won't Start**
```bash
# Check logs
docker-compose -f docker-compose-unraid.yaml logs

# Check port conflicts
netstat -tulpn | grep :7001
netstat -tulpn | grep :5432
```

### **Database Connection Issues**
```bash
# Check database container
docker logs recollect-database

# Test database connection
docker exec -it recollect-database psql -U postgres -d Recollect
```

### **API Not Responding**
```bash
# Check API container
docker logs recollect-api

# Test API endpoint
curl http://localhost:7001/admin
```

## 📊 **Monitoring**

### **Resource Usage**
```bash
# Check container resources
docker stats

# Check disk usage
df -h /mnt/user/appdata/recollect*
```

### **Logs**
```bash
# Follow all logs
docker-compose -f docker-compose-unraid.yaml logs -f

# Follow specific service
docker-compose -f docker-compose-unraid.yaml logs -f recollect-api
```

## 🔒 **Security**

### **Change Default Passwords**
1. **Edit `docker-compose-unraid.yaml`**
2. **Change `POSTGRES_PASSWORD`**
3. **Restart services**

### **Firewall Configuration**
- **Allow port 7001** for API access
- **Block port 5432** from external access

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

---

**Your Recollect Adventure Tracking System is now running on unRAID! 🚀🗺️🎭**
