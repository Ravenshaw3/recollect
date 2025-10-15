#!/bin/bash

# Build and push Recollect API to Docker Hub
# Make sure you're logged into Docker Hub: docker login

echo "Building Recollect API Docker image..."

# Build the image
docker build -f Recollect.Api/Dockerfile.dockerfile -t recollect-api:latest ./Recollect.Api

# Tag for Docker Hub (replace 'yourusername' with your Docker Hub username)
docker tag recollect-api:latest yourusername/recollect-api:latest

# Push to Docker Hub
docker push yourusername/recollect-api:latest

echo "Image pushed to Docker Hub!"
echo "You can now use 'yourusername/recollect-api:latest' in unRAID"
