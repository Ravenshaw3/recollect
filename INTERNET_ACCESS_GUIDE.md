# Recollect Internet Access Guide

This guide provides multiple options for accessing your Recollect backend from the internet, including Tailscale, reverse proxy, and direct access methods.

## 🎯 Quick Start Options

### Option 1: Tailscale (Recommended)
**Best for**: Secure, encrypted access without exposing ports
- ✅ No port forwarding needed
- ✅ Encrypted connections
- ✅ Easy device management
- ✅ Works through firewalls

**Setup**: See `tailscale-setup.md`

### Option 2: Cloudflare Tunnel
**Best for**: Easy setup with automatic SSL
- ✅ No port forwarding needed
- ✅ Automatic SSL certificates
- ✅ DDoS protection
- ✅ Global CDN

**Setup**: See `direct-internet-setup.md`

### Option 3: Direct Internet Access
**Best for**: Full control over infrastructure
- ✅ Complete control
- ✅ Custom domain
- ✅ Direct SSL management

**Setup**: See `direct-internet-setup.md`

## 🔧 Backend Updates Made

### 1. Enhanced CORS Configuration
- Added production CORS policy
- Environment-based CORS selection
- Security headers for production

### 2. Production Docker Compose
- SSL/TLS support
- Health checks
- Nginx reverse proxy
- Environment variables

### 3. Mobile App Configuration
- Dynamic API URL configuration
- Settings page for API selection
- Connection testing
- Multiple environment support

## 📱 Mobile App Features Added

### Settings Page
- API configuration selection
- Connection testing
- Custom URL support
- Real-time status updates

### Configuration Service
- Multiple environment support
- Dynamic URL switching
- Persistent settings
- Fallback configurations

## 🚀 Deployment Options

### 1. Tailscale Deployment
```bash
# Install Tailscale on unRAID
curl -fsSL https://tailscale.com/install.sh | sh
sudo tailscale up

# Get Tailscale IP
tailscale ip -4

# Update mobile app with Tailscale IP
# In ConfigurationService.cs, update the Tailscale IP
```

### 2. Production Deployment
```bash
# Run production deployment script
sudo ./deploy-production.sh your-domain.com your-email@example.com

# Get SSL certificate
sudo certbot --nginx -d your-domain.com
```

### 3. Cloudflare Tunnel
```bash
# Install cloudflared
wget https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64.deb
dpkg -i cloudflared-linux-amd64.deb

# Create tunnel
cloudflared tunnel login
cloudflared tunnel create recollect
```

## 🔒 Security Considerations

### Production Security
- Use strong passwords
- Enable HTTPS/SSL
- Configure firewall rules
- Regular security updates
- Monitor access logs

### API Security
- Rate limiting implemented
- CORS properly configured
- Security headers added
- Input validation

## 📊 Monitoring and Maintenance

### Health Checks
- API health endpoint: `/health`
- Database connectivity
- SSL certificate monitoring
- Service status monitoring

### Logs
```bash
# View API logs
docker-compose -f docker-compose-production.yaml logs -f api

# View Nginx logs
docker-compose -f docker-compose-production.yaml logs -f nginx

# View database logs
docker-compose -f docker-compose-production.yaml logs -f db
```

## 🔄 Mobile App Configuration

### Automatic Configuration
The mobile app now automatically detects and configures the API endpoint based on:
1. Environment variables
2. User preferences
3. Network conditions
4. Fallback configurations

### Manual Configuration
Users can manually configure the API endpoint through the Settings page:
1. Open the Settings tab
2. Select API configuration
3. Test connection
4. Save settings

## 🆘 Troubleshooting

### Common Issues
1. **Connection Failed**: Check firewall settings and port accessibility
2. **SSL Errors**: Verify SSL certificate configuration
3. **CORS Errors**: Check CORS policy configuration
4. **Database Connection**: Verify database credentials and connectivity

### Debug Commands
```bash
# Test API connectivity
curl -v http://your-domain.com/api/adventures

# Test SSL certificate
openssl s_client -connect your-domain.com:443

# Check Docker services
docker-compose -f docker-compose-production.yaml ps
```

## 📞 Support

For issues with internet access setup:
1. Check the troubleshooting section
2. Review logs for error messages
3. Verify network configuration
4. Test with different access methods

## 🎉 Success Indicators

Your setup is working correctly when:
- ✅ Mobile app can connect to backend
- ✅ Admin interface is accessible
- ✅ API endpoints respond correctly
- ✅ SSL certificate is valid
- ✅ No CORS errors in browser console
