# Direct Internet Access Setup

## Option 1: Reverse Proxy with Nginx

### 1. Install Nginx on unRAID
```bash
# Install Nginx via Community Applications or manually
# Create nginx configuration
```

### 2. Nginx Configuration
Create `/mnt/user/appdata/nginx/nginx.conf`:

```nginx
events {
    worker_connections 1024;
}

http {
    upstream recollect_api {
        server localhost:7001;
    }

    server {
        listen 80;
        server_name your-domain.com;  # Replace with your domain

        location / {
            proxy_pass http://recollect_api;
            proxy_set_header Host $host;
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
            proxy_set_header X-Forwarded-Proto $scheme;
        }
    }
}
```

### 3. SSL with Let's Encrypt
```bash
# Install certbot
curl -O https://dl.eff.org/certbot-auto
chmod a+x certbot-auto

# Get SSL certificate
./certbot-auto certonly --standalone -d your-domain.com
```

## Option 2: Cloudflare Tunnel

### 1. Install Cloudflared
```bash
# Download cloudflared
wget https://github.com/cloudflare/cloudflared/releases/latest/download/cloudflared-linux-amd64.deb
dpkg -i cloudflared-linux-amd64.deb
```

### 2. Create Tunnel
```bash
# Login to Cloudflare
cloudflared tunnel login

# Create tunnel
cloudflared tunnel create recollect

# Configure tunnel
cloudflared tunnel route dns recollect your-domain.com
```

### 3. Tunnel Configuration
Create `~/.cloudflared/config.yml`:

```yaml
tunnel: recollect
credentials-file: /root/.cloudflared/recollect.json

ingress:
  - hostname: your-domain.com
    service: http://localhost:7001
  - service: http_status:404
```

## Option 3: Port Forwarding (Not Recommended)

### Router Configuration
1. Access router admin panel
2. Port forwarding: External 80/443 → Internal 7001
3. Set up dynamic DNS (DuckDNS, No-IP, etc.)

### Security Considerations
- Use strong passwords
- Enable firewall rules
- Consider VPN access instead
- Regular security updates

## Mobile App Configuration

Update the mobile app to use your public domain:

```csharp
// In MauiProgram.cs
builder.Services.AddHttpClient<ApiService>(client =>
{
    client.BaseAddress = new Uri("https://your-domain.com");
});
```

## Security Best Practices

1. **Use HTTPS**: Always use SSL/TLS in production
2. **Authentication**: Add API authentication
3. **Rate Limiting**: Implement rate limiting
4. **Firewall**: Configure proper firewall rules
5. **Monitoring**: Set up monitoring and logging
