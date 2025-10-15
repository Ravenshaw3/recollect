#!/bin/bash

# Recollect API Docker Build Script for unRAID
echo "Building Recollect API Docker image..."

# Build the Docker image
docker build -t recollect-api:latest -f Recollect.Api/Dockerfile.dockerfile .

echo "Docker image built successfully!"
echo "Image: recollect-api:latest"
echo ""
echo "To deploy on unRAID:"
echo "1. Copy this project to your unRAID server"
echo "2. Run this build script"
echo "3. Import the unRAID templates"
echo "4. Start the containers"
