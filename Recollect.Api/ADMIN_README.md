# Recollect Admin Dashboard

A comprehensive web-based management interface for the Recollect backend API.

## Features

### 📊 Dashboard
- **Overview Statistics**: Total adventures, waypoints, notes, and media items
- **Recent Adventures**: Quick view of the latest adventures
- **Real-time Updates**: Live connection status and data refresh

### 🗺️ Adventures Management
- **View All Adventures**: Complete list with creation/update timestamps
- **Adventure Details**: Detailed view with waypoints, notes, and media
- **Search & Filter**: Find adventures quickly
- **Delete Adventures**: Remove unwanted adventures

### 📍 Waypoints Management
- **View All Waypoints**: List all GPS coordinates and timestamps
- **Location Details**: Latitude, longitude, and associated notes
- **Delete Waypoints**: Remove individual waypoints

### 📝 Notes Management
- **View All Notes**: Complete list of user notes
- **Note Content**: Full text and metadata
- **Location Data**: GPS coordinates if available
- **Delete Notes**: Remove individual notes

### 🖼️ Media Management
- **View All Media**: List of photos, videos, and other media
- **Media Details**: File paths, types, and captions
- **Location Data**: GPS coordinates if available
- **Delete Media**: Remove individual media items

## Accessing the Admin Interface

1. **Start the API Server**:
   ```bash
   cd Recollect.Api
   dotnet run
   ```

2. **Open Admin Dashboard**:
   Navigate to: `https://localhost:7001/admin`

## API Endpoints

The admin interface uses the following API endpoints:

### Dashboard
- `GET /api/admin/dashboard` - Get dashboard statistics

### Adventures
- `GET /api/admin/adventures` - Get all adventures
- `GET /api/admin/adventures/{id}` - Get specific adventure
- `PUT /api/admin/adventures/{id}` - Update adventure
- `DELETE /api/admin/adventures/{id}` - Delete adventure

### Waypoints
- `GET /api/admin/waypoints` - Get all waypoints
- `DELETE /api/admin/waypoints/{id}` - Delete waypoint

### Notes
- `GET /api/admin/notes` - Get all notes
- `DELETE /api/admin/notes/{id}` - Delete note

### Media
- `GET /api/admin/media` - Get all media items
- `DELETE /api/admin/media/{id}` - Delete media item

## Features

### 🎨 Modern UI
- **Responsive Design**: Works on desktop, tablet, and mobile
- **Bootstrap 5**: Modern, clean interface
- **Font Awesome Icons**: Intuitive iconography
- **Dark Mode Support**: Automatic dark mode detection

### 🔧 Admin Functions
- **Real-time Data**: Live updates from the database
- **Search & Filter**: Find data quickly
- **Bulk Operations**: Manage multiple items
- **Error Handling**: User-friendly error messages
- **Confirmation Dialogs**: Prevent accidental deletions

### 📱 Mobile Friendly
- **Responsive Layout**: Adapts to any screen size
- **Touch Friendly**: Optimized for touch devices
- **Fast Loading**: Optimized for mobile networks

## Security Considerations

⚠️ **Important**: This admin interface is designed for development and internal use. For production deployment, consider:

1. **Authentication**: Add login/authentication
2. **Authorization**: Restrict access to authorized users
3. **HTTPS**: Ensure secure connections
4. **Rate Limiting**: Prevent abuse
5. **Audit Logging**: Track admin actions

## Development

### File Structure
```
Recollect.Api/
├── Controllers/
│   └── AdminController.cs          # Admin API endpoints
├── wwwroot/admin/
│   ├── index.html                  # Main admin interface
│   ├── default.html                # Default redirect
│   ├── styles.css                  # Custom styling
│   └── admin.js                    # JavaScript functionality
└── Program.cs                      # Updated to serve admin files
```

### Customization

You can customize the admin interface by:

1. **Modifying Styles**: Edit `wwwroot/admin/styles.css`
2. **Adding Features**: Extend `wwwroot/admin/admin.js`
3. **New Endpoints**: Add methods to `AdminController.cs`
4. **UI Components**: Modify `wwwroot/admin/index.html`

## Troubleshooting

### Common Issues

1. **Admin Interface Not Loading**:
   - Ensure the API server is running
   - Check that static files are being served
   - Verify the URL: `https://localhost:7001/admin`

2. **API Errors**:
   - Check browser console for JavaScript errors
   - Verify database connection
   - Check API endpoint responses

3. **Data Not Loading**:
   - Ensure database is properly configured
   - Check CORS settings
   - Verify API endpoints are accessible

### Browser Compatibility

- **Chrome**: Full support
- **Firefox**: Full support
- **Safari**: Full support
- **Edge**: Full support
- **Mobile Browsers**: Responsive design supported

## Future Enhancements

Potential improvements for the admin interface:

1. **User Authentication**: Login system
2. **Role-based Access**: Different permission levels
3. **Data Export**: Export adventures to various formats
4. **Bulk Operations**: Select and manage multiple items
5. **Advanced Search**: More sophisticated filtering
6. **Analytics**: Usage statistics and insights
7. **Backup/Restore**: Database management tools
8. **Real-time Updates**: WebSocket integration
