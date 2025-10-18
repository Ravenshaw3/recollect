#!/bin/bash

# Production Deployment Script for Recollect
# This script deploys the Recollect system with internet access

set -e

echo "🚀 Starting Recollect Production Deployment..."

# Check if running as root
if [ "$EUID" -ne 0 ]; then
    echo "Please run as root (use sudo)"
    exit 1
fi

# Configuration
DOMAIN=${1:-"your-domain.com"}
EMAIL=${2:-"your-email@example.com"}
POSTGRES_PASSWORD=${3:-$(openssl rand -base64 32)}

echo "📋 Configuration:"
echo "  Domain: $DOMAIN"
echo "  Email: $EMAIL"
echo "  PostgreSQL Password: [HIDDEN]"

# Create directories
echo "📁 Creating directories..."
mkdir -p ssl
mkdir -p nginx
mkdir -p data/postgres

# Generate SSL certificates (self-signed for now)
echo "🔐 Generating SSL certificates..."
if [ ! -f ssl/cert.pem ]; then
    openssl req -x509 -newkey rsa:4096 -keyout ssl/key.pem -out ssl/cert.pem -days 365 -nodes \
        -subj "/C=US/ST=State/L=City/O=Organization/CN=$DOMAIN"
    echo "✅ SSL certificates generated"
else
    echo "ℹ️  SSL certificates already exist"
fi

# Create environment file
echo "📝 Creating environment file..."
cat > production.env << EOF
ASPNETCORE_ENVIRONMENT=Production
POSTGRES_PASSWORD=$POSTGRES_PASSWORD
SSL_CERT_PASSWORD=
API_BASE_URL=https://$DOMAIN
API_TIMEOUT=30
JWT_SECRET=$(openssl rand -base64 32)
ENCRYPTION_KEY=$(openssl rand -base64 32)
ENABLE_LOGGING=true
LOG_LEVEL=Information
ENABLE_METRICS=true
RATE_LIMIT_REQUESTS_PER_MINUTE=100
RATE_LIMIT_BURST=20
EOF

# Update nginx configuration with domain
echo "🔧 Configuring Nginx..."
sed "s/your-domain.com/$DOMAIN/g" nginx/nginx.conf > nginx/nginx-production.conf

# Build and start services
echo "🐳 Building and starting Docker services..."
docker-compose -f docker-compose-production.yaml --env-file production.env up -d --build

# Wait for services to be ready
echo "⏳ Waiting for services to start..."
sleep 30

# Test the deployment
echo "🧪 Testing deployment..."
if curl -f -s http://localhost:7001/health > /dev/null; then
    echo "✅ API is responding"
else
    echo "❌ API is not responding"
    exit 1
fi

if curl -f -s http://localhost > /dev/null; then
    echo "✅ Nginx is responding"
else
    echo "❌ Nginx is not responding"
    exit 1
fi

echo "🎉 Deployment completed successfully!"
echo ""
echo "📋 Next steps:"
echo "1. Configure your domain DNS to point to this server"
echo "2. Get a real SSL certificate with Let's Encrypt:"
echo "   sudo certbot --nginx -d $DOMAIN"
echo "3. Update your mobile app with the domain: $DOMAIN"
echo "4. Access the admin interface at: https://$DOMAIN/admin"
echo ""
echo "🔧 Management commands:"
echo "  View logs: docker-compose -f docker-compose-production.yaml logs -f"
echo "  Stop services: docker-compose -f docker-compose-production.yaml down"
echo "  Restart services: docker-compose -f docker-compose-production.yaml restart"
