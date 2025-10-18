# Tailscale Quick Setup for Recollect

Since Tailscale is already installed on your unRAID server, follow these steps to get Recollect working with Tailscale access.

## 🚀 Quick Deployment

### 1. Deploy with Tailscale
```bash
# SSH into your unRAID server
ssh root@your-unraid-ip

# Navigate to your Recollect directory
cd /path/to/recollect

# Run the Tailscale deployment script
sudo ./deploy-tailscale.sh
```

### 2. Get Your Tailscale IP
The script will automatically detect and display your Tailscale IP. It will look something like:
```
✅ Tailscale IP: 100.64.0.123
```

## 📱 Mobile App Configuration

### 1. Update ConfigurationService.cs
Replace `YOUR_TAILSCALE_IP` in `Recollect.Mobile/Services/ConfigurationService.cs` with your actual Tailscale IP:

```csharp
// Replace this line:
["tailscale"] = "http://YOUR_TAILSCALE_IP:7001",

// With your actual IP (example):
["tailscale"] = "http://100.64.0.123:7001",
```

### 2. Build and Deploy Mobile App
```bash
# Build the mobile app
dotnet build Recollect.Mobile/Recollect.Mobile.csproj -c Release -f net9.0-android

# Or use the build script
.\build-with-api-key.ps1 -GoogleMapsApiKey "your-api-key"
```

### 3. Configure Mobile App
1. Install the mobile app on your device
2. Open the app and go to the **Settings** tab
3. Select **"tailscale"** from the API Configuration dropdown
4. Tap **"Test Connection"** to verify it works
5. The app will now use your Tailscale IP for all API calls

## 🌐 Access from Other Devices

### 1. Install Tailscale on Mobile Device
- Download Tailscale from your app store
- Sign in with the same account as your unRAID server
- Join the same Tailscale network

### 2. Access Admin Interface
- Open a web browser on any device with Tailscale
- Navigate to: `http://YOUR_TAILSCALE_IP:7001/admin`
- You'll have full access to the admin interface

### 3. Test API Access
```bash
# Test from any device with Tailscale
curl http://YOUR_TAILSCALE_IP:7001/api/adventures
```

## 🔧 Management Commands

### View Logs
```bash
docker-compose -f docker-compose-tailscale.yaml logs -f
```

### Restart Services
```bash
docker-compose -f docker-compose-tailscale.yaml restart
```

### Stop Services
```bash
docker-compose -f docker-compose-tailscale.yaml down
```

### Update Services
```bash
docker-compose -f docker-compose-tailscale.yaml pull
docker-compose -f docker-compose-tailscale.yaml up -d
```

## 🆘 Troubleshooting

### Common Issues

1. **Can't connect from mobile app**
   - Ensure Tailscale is installed and connected on your mobile device
   - Check that you're using the correct Tailscale IP
   - Verify the mobile app is using the "tailscale" configuration

2. **API not responding**
   - Check if Docker containers are running: `docker ps`
   - Check logs: `docker-compose -f docker-compose-tailscale.yaml logs -f api`
   - Verify Tailscale IP is correct

3. **Admin interface not accessible**
   - Try accessing: `http://YOUR_TAILSCALE_IP:7001/admin`
   - Check firewall settings on unRAID
   - Verify Tailscale is working: `tailscale status`

### Debug Commands
```bash
# Check Tailscale status
tailscale status

# Test API locally
curl http://localhost:7001/health

# Test API via Tailscale
curl http://YOUR_TAILSCALE_IP:7001/health

# Check Docker services
docker-compose -f docker-compose-tailscale.yaml ps
```

## ✅ Success Indicators

Your setup is working correctly when:
- ✅ `tailscale status` shows your server as online
- ✅ `curl http://YOUR_TAILSCALE_IP:7001/health` returns success
- ✅ Mobile app can connect using "tailscale" configuration
- ✅ Admin interface is accessible at `http://YOUR_TAILSCALE_IP:7001/admin`

## 🎉 Next Steps

Once everything is working:
1. **Test the mobile app** - Create an adventure and add waypoints
2. **Access admin interface** - View your adventures from any device
3. **Share with family** - Add them to your Tailscale network for shared access
4. **Backup your data** - The database is stored in Docker volumes

## 📞 Support

If you encounter issues:
1. Check the troubleshooting section above
2. Review the logs for error messages
3. Verify Tailscale connectivity
4. Test with different devices on the Tailscale network
