# 🗺️ Recollect - Adventure Tracking System

> Transform your real adventures into epic animated stories with AI-powered humor!

[![.NET](https://img.shields.io/badge/.NET-9.0-blue.svg)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-Ready-2496ED.svg)](https://www.docker.com/)
[![unRAID](https://img.shields.io/badge/unRAID-Compatible-FF6B35.svg)](https://unraid.net/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

## 🎯 Overview

Recollect is a comprehensive adventure tracking system that combines GPS data collection, web-based administration, and AI-powered story generation. Transform your real hiking, biking, or travel adventures into humorous, animated stories that bring your experiences to life!

## ✨ Features

### 🗺️ **Adventure Tracking**
- **GPS Waypoint Collection**: Record precise location data during adventures
- **Rich Media Support**: Attach photos, videos, and notes to waypoints
- **Real-time Tracking**: Live adventure monitoring and data collection
- **Adventure Statistics**: Distance, duration, elevation, and route analysis

### 🎭 **AI Story Generator**
- **Animated Stories**: Transform real GPS data into epic animated narratives
- **Humorous AI**: AI-powered humor that turns your adventures into comedy gold
- **Interactive Viewer**: Animated story playback with sound effects
- **Random Story Mode**: Generate stories from any adventure in your collection

### 🖥️ **Admin Dashboard**
- **Web-based Management**: Complete admin interface for adventure data
- **Real-time Statistics**: Adventure metrics and performance analytics
- **Media Management**: Organize photos, videos, and notes
- **Story Generation**: Create and manage animated adventure stories

### 🐳 **Docker Deployment**
- **Container Ready**: Full Docker support with docker-compose
- **unRAID Templates**: Ready-to-use unRAID Docker templates
- **Production Ready**: Optimized for production deployment
- **Easy Setup**: One-command deployment

## 🚀 Quick Start

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download)
- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

### Local Development

1. **Clone the repository**
   ```bash
   git clone https://github.com/Ravenshaw3/recollect.git
   cd recollect
   ```

2. **Start the services**
   ```bash
   docker-compose up -d
   ```

3. **Access the admin dashboard**
   - Open [http://localhost:7001/admin](http://localhost:7001/admin)
   - Start creating adventures and generating stories!

### 🐳 Docker Deployment

```bash
# Build and start all services
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down
```

## 📱 Mobile App

The system includes a MAUI mobile app (`Recollect.Mobile`) for:
- **Live GPS Tracking**: Record adventures in real-time
- **Media Capture**: Take photos and videos during adventures
- **Offline Support**: Continue tracking without internet connection
- **Sync with Backend**: Automatic data synchronization

## 🎭 Story Generator

### How It Works
1. **Collect Adventure Data**: Use the mobile app to record your adventure
2. **Access Admin Dashboard**: View your adventures in the web interface
3. **Generate Stories**: Click "Story Generator" to create animated stories
4. **Watch Magic Happen**: AI transforms your GPS data into humorous narratives!

### Story Features
- **AI-Powered Humor**: Intelligent story generation based on your data
- **Animated Playback**: Visual effects and animations
- **Sound Effects**: Audio feedback for story elements
- **Statistics Integration**: Adventure metrics woven into the narrative
- **Random Mode**: Generate stories from any adventure

## 🏗️ Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Mobile App    │    │   Admin UI      │    │   Story Viewer  │
│   (MAUI)        │    │   (Web)         │    │   (Animated)    │
└─────────┬───────┘    └─────────┬───────┘    └─────────┬───────┘
          │                      │                      │
          └──────────────────────┼──────────────────────┘
                                 │
                    ┌─────────────▼─────────────┐
                    │      .NET API            │
                    │   (Recollect.Api)        │
                    └─────────────┬─────────────┘
                                  │
                    ┌─────────────▼─────────────┐
                    │   PostgreSQL    │
                    └─────────────────────────────┘
```

## 📊 API Endpoints

### Adventures
- `GET /api/adventures` - Get all adventures
- `GET /api/adventures/{id}` - Get specific adventure
- `POST /api/adventures/upload` - Upload new adventure
- `DELETE /api/adventures/{id}` - Delete adventure

### Story Generation
- `GET /api/story/{adventureId}` - Generate story for adventure
- `GET /api/story/random` - Generate random story
- `GET /api/story/stats/{adventureId}` - Get adventure statistics

### Admin
- `GET /admin` - Admin dashboard
- `GET /admin/story.html` - Story generator interface

## 🐳 unRAID Deployment

### Using Docker Templates

1. **Download Templates**:
   - `unraid-docker-template.xml` - API container
   - `unraid-docker-template-db.xml` - PostgreSQL database

2. **Install in unRAID**:
   - Go to Docker tab in unRAID
   - Add Container → Add from template
   - Upload the XML files
   - Configure ports and settings

3. **Access Your System**:
   - API: `http://your-unraid-ip:7001/admin`
   - Database: `your-unraid-ip:5432`

## 🎨 Custom Icons

The project includes custom SVG icons for unRAID:
- `icons/recollect-api-icon.svg` - API container icon
- `icons/recollect-db-icon.svg` - Database container icon
- `icons/create-icons.html` - Icon preview and conversion tool

## 📁 Project Structure

```
recollect/
├── Recollect.Api/                 # .NET Web API
│   ├── Controllers/              # API controllers
│   ├── Models/                   # Data models
│   ├── wwwroot/admin/           # Admin dashboard
│   └── Dockerfile.dockerfile    # Docker configuration
├── Recollect.Mobile/            # MAUI mobile app
├── docker-compose.yaml          # Docker services
├── unraid-docker-template.xml   # unRAID API template
├── unraid-docker-template-db.xml # unRAID DB template
├── icons/                       # Custom icons
└── docs/                        # Documentation
```

## 🛠️ Development

### Adding New Features
1. **Backend**: Add controllers in `Recollect.Api/Controllers/`
2. **Frontend**: Update admin UI in `Recollect.Api/wwwroot/admin/`
3. **Mobile**: Extend MAUI app in `Recollect.Mobile/`
4. **Docker**: Update `docker-compose.yaml` for new services

### Testing
```bash
# Run API tests
cd Recollect.Api
dotnet test

# Run mobile tests
cd Recollect.Mobile
dotnet test
```

## 📝 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## 🐛 Issues & Support

- **Bug Reports**: [Create an issue](https://github.com/Ravenshaw3/recollect/issues)
- **Feature Requests**: [Start a discussion](https://github.com/Ravenshaw3/recollect/discussions)
- **Documentation**: Check the `docs/` folder for detailed guides

## 🎉 Acknowledgments

- **.NET Team** for the amazing framework
- **MAUI Team** for cross-platform mobile development
- **Docker Team** for containerization
- **unRAID Community** for server deployment support

---

**Made with ❤️ for adventurers who love to tell their stories!**

[![GitHub stars](https://img.shields.io/github/stars/Ravenshaw3/recollect?style=social)](https://github.com/Ravenshaw3/recollect)
[![GitHub forks](https://img.shields.io/github/forks/Ravenshaw3/recollect?style=social)](https://github.com/Ravenshaw3/recollect)
