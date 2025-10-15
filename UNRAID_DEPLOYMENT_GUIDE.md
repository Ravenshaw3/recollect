# Recollect API - unRAID Docker Deployment Guide

## 🐳 **Docker Deployment on unRAID**

This guide will help you deploy the Recollect API with admin dashboard on your unRAID server using Docker containers.

## 📋 **Prerequisites**

- unRAID server with Docker support
- Access to unRAID web interface
- Basic understanding of Docker containers

## 🚀 **Deployment Options**

### **Option 1: Docker Compose (Recommended)**

1. **Upload Project to unRAID**:
   ```bash
   # Copy your project to unRAID server
   # Place in: /mnt/user/appdata/recollect/
   ```

2. **Deploy with Docker Compose**:
   ```bash
   cd /mnt/user/appdata/recollect/
   docker-compose up -d
   ```

3. **Access the API**:
   - **API**: `http://your-unraid-ip:7001`
   - **Admin Dashboard**: `http://your-unraid-ip:7001/admin`
   - **Database**: `your-unraid-ip:5432`

### **Option 2: unRAID Docker Templates**

1. **Import Database Template**:
   - Go to Docker tab in unRAID
   - Click "Add Container"
   - Select "Recollect-Database" template
   - Configure database settings
   - Start the database container

2. **Import API Template**:
   - Go to Docker tab in unRAID
   - Click "Add Container"
   - Select "Recollect-API" template
   - Configure API settings
   - Start the API container

## 🔧 **Configuration**

### **Database Configuration**
- **Database Name**: Recollect
- **Username**: postgres
- **Password**: secret (change for production)
- **Port**: 5432

### **API Configuration**
- **HTTP Port**: 7001
- **HTTPS Port**: 7002
- **Environment**: Production
- **Data Directory**: `/mnt/user/appdata/recollect`

## 📁 **Directory Structure**

```
/mnt/user/appdata/
├── recollect/                 # API application
│   ├── Recollect.Api/
│   ├── docker-compose.yaml
│   └── wwwroot/admin/         # Admin dashboard files
└── recollect-db/             # Database data
```

## 🌐 **Access Points**

### **API Endpoints**
- **Base API**: `http://your-unraid-ip:7001/api/`
- **Adventures**: `http://your-unraid-ip:7001/api/adventures`
- **Admin API**: `http://your-unraid-ip:7001/api/admin/`

### **Admin Dashboard**
- **Main Dashboard**: `http://your-unraid-ip:7001/admin`
- **Features**:
  - Adventure management
  - Waypoint tracking
  - Notes management
  - Media management
  - Real-time statistics

## 🔒 **Security Considerations**

### **Production Setup**
1. **Change Default Passwords**:
   ```bash
   # Update database password
   POSTGRES_PASSWORD=your-secure-password
   ```

2. **Enable HTTPS**:
   - Configure SSL certificates
   - Update ports for HTTPS (7002)

3. **Network Security**:
   - Use unRAID firewall rules
   - Restrict database access
   - Consider VPN access

## 📊 **Monitoring & Maintenance**

### **Container Health**
```bash
# Check container status
docker ps

# View logs
docker logs recollect-api
docker logs recollect-database
```

### **Database Backup**
```bash
# Backup database
docker exec recollect-database pg_dump -U postgres Recollect > backup.sql

# Restore database
docker exec -i recollect-database psql -U postgres Recollect < backup.sql
```

### **Updates**
```bash
# Update containers
docker-compose pull
docker-compose up -d
```

## 🛠️ **Troubleshooting**

### **Common Issues**

1. **Port Conflicts**:
   - Check if ports 7001, 7002, 5432 are available
   - Update port mappings in docker-compose.yaml

2. **Database Connection**:
   - Verify database container is running
   - Check connection string in API configuration

3. **Permission Issues**:
   - Ensure proper directory permissions
   - Check unRAID user permissions

### **Logs & Debugging**
```bash
# API logs
docker logs recollect-api

# Database logs
docker logs recollect-database

# Container status
docker inspect recollect-api
```

## 📱 **Mobile App Integration**

Your MAUI mobile app can connect to the API using:
```csharp
// Update API base URL in your mobile app
var apiBaseUrl = "http://your-unraid-ip:7001/api/";
```

## 🔄 **Backup Strategy**

### **Automated Backups**
1. **Database Backups**:
   - Schedule daily database dumps
   - Store in unRAID backup location

2. **Application Data**:
   - Backup `/mnt/user/appdata/recollect/`
   - Include admin dashboard files

3. **Docker Images**:
   - Export container images
   - Store configuration files

## 📈 **Scaling & Performance**

### **Resource Allocation**
- **API Container**: 1-2 CPU cores, 1-2GB RAM
- **Database Container**: 1-2 CPU cores, 2-4GB RAM

### **Performance Optimization**
- Enable database connection pooling
- Configure caching
- Monitor resource usage

## 🆘 **Support**

If you encounter issues:
1. Check container logs
2. Verify network connectivity
3. Ensure proper permissions
4. Review unRAID system logs

## 📝 **Next Steps**

1. **Deploy the containers**
2. **Access the admin dashboard**
3. **Configure your mobile app**
4. **Set up automated backups**
5. **Monitor performance**

Your Recollect API with admin dashboard is now ready for deployment on unRAID! 🎉
