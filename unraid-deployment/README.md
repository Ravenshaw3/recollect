# 🚀 unRAID Docker Compose Deployment

## 📋 **Quick Start**

1. **Copy these files to your unRAID server**
2. **SSH into your unRAID server**
3. **Run**: `docker-compose -f docker-compose-unraid.yaml up -d`
4. **Access**: `http://YOUR-UNRAID-IP:7001/admin`

## 📁 **Files Included**
- `docker-compose-unraid.yaml` - Main deployment file
- `build-api.sh` - Script to build the API image
- `README.md` - This guide

## 🔧 **Configuration**
- **API Port**: 7001
- **Database Port**: 5432 (internal)
- **Data Directory**: `/mnt/user/appdata/recollect`
- **Database Directory**: `/mnt/user/appdata/recollect-db`

## 🎭 **Features**
- Adventure tracking API
- Admin dashboard
- AI story generator
- PostgreSQL database
- Automatic restart on failure
