#!/bin/bash

# Build Recollect API Docker image for unRAID deployment
echo "🚀 Building Recollect API for unRAID..."

# Create build context
mkdir -p build-context
cp -r ../Recollect.Api/* build-context/

# Build the Docker image
docker build -f build-context/Dockerfile.dockerfile -t recollect-api:latest build-context/

# Clean up
rm -rf build-context

echo "✅ Recollect API image built successfully!"
echo "📦 Image: recollect-api:latest"
echo "🎯 Ready for unRAID deployment!"
