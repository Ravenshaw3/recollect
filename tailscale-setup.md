# Tailscale Setup for Recollect Backend Access

## Overview
Tailscale provides secure, encrypted access to your Recollect backend from anywhere on the internet without exposing ports directly.

## Setup Steps

### 1. Install Tailscale on unRAID Server
```bash
# SSH into your unRAID server
ssh root@your-unraid-ip

# Install Tailscale
curl -fsSL https://tailscale.com/install.sh | sh

# Start Tailscale
sudo tailscale up

# Get your Tailscale IP
tailscale ip -4
```

### 2. Configure Docker Compose for Tailscale
Create `docker-compose-tailscale.yaml`:

```yaml
version: '3.8'
services:
  api:
    build: 
      context: ./Recollect.Api
      dockerfile: Dockerfile.dockerfile
    ports:
      - "7001:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:80
      - ConnectionStrings__DefaultConnection=Host=db;Database=Recollect;Username=postgres;Password=secret
      - TAILSCALE_IP=${TAILSCALE_IP}
    depends_on:
      - db
    restart: unless-stopped
    networks:
      - recollect-network
    extra_hosts:
      - "host.docker.internal:host-gateway"
  db:
    image: postgres:16
    environment:
      POSTGRES_DB: Recollect
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: secret
    volumes:
      - postgres_data:/var/lib/postgresql/data
    restart: unless-stopped
    networks:
      - recollect-network

volumes:
  postgres_data:

networks:
  recollect-network:
    driver: bridge
```

### 3. Update Mobile App Configuration
Update `Recollect.Mobile/Services/ApiService.cs` to use Tailscale IP:

```csharp
// In MauiProgram.cs, update the HttpClient configuration:
builder.Services.AddHttpClient<ApiService>(client =>
{
    // Replace with your Tailscale IP
    client.BaseAddress = new Uri("http://YOUR-TAILSCALE-IP:7001");
});
```

### 4. Access Control Lists (ACLs)
Create `/etc/tailscale/acls.json`:

```json
{
  "acls": [
    {
      "action": "accept",
      "src": ["autogroup:members"],
      "dst": ["*:7001"]
    }
  ]
}
```

## Benefits of Tailscale
- ✅ No port forwarding required
- ✅ Encrypted connections
- ✅ Easy device management
- ✅ Works through firewalls/NAT
- ✅ No dynamic DNS needed

## Alternative: Direct Internet Access
If you prefer direct internet access, see `direct-internet-setup.md`
