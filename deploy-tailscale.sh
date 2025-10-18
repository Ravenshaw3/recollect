#!/bin/bash

# Tailscale Deployment Script for Recollect
# This script deploys the Recollect system with Tailscale access

set -e

echo "🚀 Starting Recollect Tailscale Deployment..."

# Check if running as root
if [ "$EUID" -ne 0 ]; then
    echo "Please run as root (use sudo)"
    exit 1
fi

# Get Tailscale IP
echo "🔍 Getting Tailscale IP..."
TAILSCALE_IP=$(tailscale ip -4)
if [ -z "$TAILSCALE_IP" ]; then
    echo "❌ Could not get Tailscale IP. Make sure Tailscale is running."
    exit 1
fi

echo "✅ Tailscale IP: $TAILSCALE_IP"

# Create environment file for Tailscale
echo "📝 Creating Tailscale environment file..."
cat > tailscale.env << EOF
ASPNETCORE_ENVIRONMENT=Production
POSTGRES_PASSWORD=secret
TAILSCALE_IP=$TAILSCALE_IP
API_BASE_URL=http://$TAILSCALE_IP:7001
EOF

# Build and start services
echo "🐳 Building and starting Docker services..."
docker-compose -f docker-compose-tailscale.yaml --env-file tailscale.env up -d --build

# Wait for services to be ready
echo "⏳ Waiting for services to start..."
sleep 30

# Test the deployment
echo "🧪 Testing deployment..."
if curl -f -s http://localhost:7001/health > /dev/null; then
    echo "✅ API is responding locally"
else
    echo "❌ API is not responding locally"
    exit 1
fi

# Test Tailscale access
echo "🧪 Testing Tailscale access..."
if curl -f -s http://$TAILSCALE_IP:7001/health > /dev/null; then
    echo "✅ API is responding via Tailscale"
else
    echo "⚠️  API not responding via Tailscale (this might be normal if testing from same machine)"
fi

echo "🎉 Tailscale deployment completed successfully!"
echo ""
echo "📋 Configuration Summary:"
echo "  Tailscale IP: $TAILSCALE_IP"
echo "  API URL: http://$TAILSCALE_IP:7001"
echo "  Admin Interface: http://$TAILSCALE_IP:7001/admin"
echo ""
echo "📱 Mobile App Configuration:"
echo "1. Update ConfigurationService.cs with your Tailscale IP: $TAILSCALE_IP"
echo "2. Build and deploy the mobile app"
echo "3. In the mobile app, go to Settings and select 'tailscale' configuration"
echo ""
echo "🔧 Management commands:"
echo "  View logs: docker-compose -f docker-compose-tailscale.yaml logs -f"
echo "  Stop services: docker-compose -f docker-compose-tailscale.yaml down"
echo "  Restart services: docker-compose -f docker-compose-tailscale.yaml restart"
echo ""
echo "🌐 Access from other devices:"
echo "1. Install Tailscale on your mobile device"
echo "2. Join the same Tailscale network"
echo "3. Use the API URL: http://$TAILSCALE_IP:7001"
