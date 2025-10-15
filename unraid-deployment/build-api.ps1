# Build Recollect API Docker image for unRAID deployment
Write-Host "🚀 Building Recollect API for unRAID..." -ForegroundColor Green

# Create build context
New-Item -ItemType Directory -Path "build-context" -Force
Copy-Item -Path "../Recollect.Api/*" -Destination "build-context/" -Recurse

# Build the Docker image
docker build -f build-context/Dockerfile.dockerfile -t recollect-api:latest build-context/

# Clean up
Remove-Item -Path "build-context" -Recurse -Force

Write-Host "✅ Recollect API image built successfully!" -ForegroundColor Green
Write-Host "📦 Image: recollect-api:latest" -ForegroundColor Cyan
Write-Host "🎯 Ready for unRAID deployment!" -ForegroundColor Yellow
