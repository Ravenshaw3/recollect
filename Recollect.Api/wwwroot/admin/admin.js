// Recollect Admin Dashboard JavaScript
const API_BASE = '/api/admin';
let currentAdventureId = null;

// Initialize the application
document.addEventListener('DOMContentLoaded', function() {
    loadDashboard();
    setupEventListeners();
});

function setupEventListeners() {
    // Search functionality
    const adventureSearch = document.getElementById('adventureSearch');
    if (adventureSearch) {
        adventureSearch.addEventListener('input', debounce(filterAdventures, 300));
    }
}

function debounce(func, wait) {
    let timeout;
    return function executedFunction(...args) {
        const later = () => {
            clearTimeout(timeout);
            func(...args);
        };
        clearTimeout(timeout);
        timeout = setTimeout(later, wait);
    };
}

// Navigation functions
function showDashboard() {
    hideAllSections();
    document.getElementById('dashboard-section').style.display = 'block';
    updateActiveNav('dashboard');
    loadDashboard();
}

function showAdventures() {
    hideAllSections();
    document.getElementById('adventures-section').style.display = 'block';
    updateActiveNav('adventures');
    loadAdventures();
}

function showWaypoints() {
    hideAllSections();
    document.getElementById('waypoints-section').style.display = 'block';
    updateActiveNav('waypoints');
    loadWaypoints();
}

function showNotes() {
    hideAllSections();
    document.getElementById('notes-section').style.display = 'block';
    updateActiveNav('notes');
    loadNotes();
}

function showMedia() {
    hideAllSections();
    document.getElementById('media-section').style.display = 'block';
    updateActiveNav('media');
    loadMedia();
}

function showStoryGenerator() {
    hideAllSections();
    document.getElementById('story-section').style.display = 'block';
    updateActiveNav('story');
    loadStoryAdventures();
}

function hideAllSections() {
    const sections = document.querySelectorAll('.content-section');
    sections.forEach(section => section.style.display = 'none');
}

function updateActiveNav(activeSection) {
    const navLinks = document.querySelectorAll('.nav-link');
    navLinks.forEach(link => link.classList.remove('active'));
    
    const activeLink = document.querySelector(`[onclick="show${activeSection.charAt(0).toUpperCase() + activeSection.slice(1)}()"]`);
    if (activeLink) {
        activeLink.classList.add('active');
    }
}

// Dashboard functions
async function loadDashboard() {
    try {
        showLoading('recentAdventures');
        const response = await fetch(`${API_BASE}/dashboard`);
        const data = await response.json();
        
        if (response.ok) {
            updateDashboardStats(data);
            displayRecentAdventures(data.RecentAdventures);
        } else {
            showError('Failed to load dashboard data');
        }
    } catch (error) {
        console.error('Error loading dashboard:', error);
        showError('Failed to connect to server');
        updateConnectionStatus(false);
    }
}

function updateDashboardStats(data) {
    document.getElementById('totalAdventures').textContent = data.TotalAdventures;
    document.getElementById('totalWaypoints').textContent = data.TotalWaypoints;
    document.getElementById('totalNotes').textContent = data.TotalNotes;
    document.getElementById('totalMedia').textContent = data.TotalMediaItems;
}

function displayRecentAdventures(adventures) {
    const container = document.getElementById('recentAdventures');
    
    if (!adventures || adventures.length === 0) {
        container.innerHTML = `
            <div class="empty-state">
                <i class="fas fa-route"></i>
                <p>No adventures found</p>
            </div>
        `;
        return;
    }
    
    container.innerHTML = adventures.map(adventure => `
        <div class="adventure-item" onclick="viewAdventure(${adventure.Id})">
            <div class="adventure-header">
                <h6 class="adventure-title">${escapeHtml(adventure.Name)}</h6>
                <small class="adventure-date">${formatDate(adventure.CreatedAt)}</small>
            </div>
            <div class="adventure-stats">
                <span class="stat-badge">ID: ${adventure.Id}</span>
            </div>
        </div>
    `).join('');
}

// Adventures functions
async function loadAdventures() {
    try {
        showLoading('adventuresList');
        const response = await fetch(`${API_BASE}/adventures`);
        const data = await response.json();
        
        if (response.ok) {
            displayAdventures(data);
        } else {
            showError('Failed to load adventures');
        }
    } catch (error) {
        console.error('Error loading adventures:', error);
        showError('Failed to connect to server');
    }
}

function displayAdventures(adventures) {
    const container = document.getElementById('adventuresList');
    
    if (!adventures || adventures.length === 0) {
        container.innerHTML = `
            <div class="empty-state">
                <i class="fas fa-route"></i>
                <p>No adventures found</p>
            </div>
        `;
        return;
    }
    
    container.innerHTML = adventures.map(adventure => `
        <div class="adventure-item">
            <div class="adventure-header">
                <h6 class="adventure-title">${escapeHtml(adventure.Name)}</h6>
                <div>
                    <button class="btn btn-sm btn-outline-primary me-2" onclick="viewAdventure(${adventure.Id})">
                        <i class="fas fa-eye"></i> View
                    </button>
                    <button class="btn btn-sm btn-outline-danger" onclick="deleteAdventure(${adventure.Id})">
                        <i class="fas fa-trash"></i> Delete
                    </button>
                </div>
            </div>
            <div class="adventure-stats">
                <span class="stat-badge">Created: ${formatDate(adventure.CreatedAt)}</span>
                <span class="stat-badge">Updated: ${formatDate(adventure.UpdatedAt)}</span>
                <span class="stat-badge">Waypoints: ${adventure.Waypoints?.length || 0}</span>
                <span class="stat-badge">Notes: ${adventure.Notes?.length || 0}</span>
                <span class="stat-badge">Media: ${adventure.MediaItems?.length || 0}</span>
            </div>
        </div>
    `).join('');
}

async function viewAdventure(id) {
    try {
        const response = await fetch(`${API_BASE}/adventures/${id}`);
        const adventure = await response.json();
        
        if (response.ok) {
            currentAdventureId = id;
            showAdventureModal(adventure);
        } else {
            showError('Failed to load adventure details');
        }
    } catch (error) {
        console.error('Error loading adventure:', error);
        showError('Failed to connect to server');
    }
}

function showAdventureModal(adventure) {
    const modalBody = document.getElementById('adventureModalBody');
    modalBody.innerHTML = `
        <div class="row">
            <div class="col-md-6">
                <h6>Basic Information</h6>
                <p><strong>Name:</strong> ${escapeHtml(adventure.Name)}</p>
                <p><strong>Created:</strong> ${formatDate(adventure.CreatedAt)}</p>
                <p><strong>Updated:</strong> ${formatDate(adventure.UpdatedAt)}</p>
            </div>
            <div class="col-md-6">
                <h6>Statistics</h6>
                <p><strong>Waypoints:</strong> ${adventure.Waypoints?.length || 0}</p>
                <p><strong>Notes:</strong> ${adventure.Notes?.length || 0}</p>
                <p><strong>Media Items:</strong> ${adventure.MediaItems?.length || 0}</p>
            </div>
        </div>
        
        ${adventure.Waypoints && adventure.Waypoints.length > 0 ? `
        <div class="mt-4">
            <h6>Waypoints</h6>
            <div class="table-responsive">
                <table class="table table-sm">
                    <thead>
                        <tr>
                            <th>Latitude</th>
                            <th>Longitude</th>
                            <th>Timestamp</th>
                            <th>Note</th>
                        </tr>
                    </thead>
                    <tbody>
                        ${adventure.Waypoints.map(wp => `
                            <tr>
                                <td>${wp.Latitude}</td>
                                <td>${wp.Longitude}</td>
                                <td>${formatDate(wp.Timestamp)}</td>
                                <td>${escapeHtml(wp.Note || '')}</td>
                            </tr>
                        `).join('')}
                    </tbody>
                </table>
            </div>
        </div>
        ` : ''}
        
        ${adventure.Notes && adventure.Notes.length > 0 ? `
        <div class="mt-4">
            <h6>Notes</h6>
            ${adventure.Notes.map(note => `
                <div class="card mb-2">
                    <div class="card-body">
                        <h6 class="card-title">${escapeHtml(note.Title)}</h6>
                        <p class="card-text">${escapeHtml(note.Content)}</p>
                        <small class="text-muted">${formatDate(note.Timestamp)}</small>
                    </div>
                </div>
            `).join('')}
        </div>
        ` : ''}
    `;
    
    const modal = new bootstrap.Modal(document.getElementById('adventureModal'));
    modal.show();
}

async function deleteAdventure(id) {
    if (!confirm('Are you sure you want to delete this adventure? This action cannot be undone.')) {
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE}/adventures/${id}`, {
            method: 'DELETE'
        });
        
        if (response.ok) {
            showSuccess('Adventure deleted successfully');
            loadAdventures();
            loadDashboard(); // Refresh dashboard stats
        } else {
            const error = await response.json();
            showError(error.Error || 'Failed to delete adventure');
        }
    } catch (error) {
        console.error('Error deleting adventure:', error);
        showError('Failed to connect to server');
    }
}

// Waypoints functions
async function loadWaypoints() {
    try {
        showLoading('waypointsList');
        const response = await fetch(`${API_BASE}/waypoints`);
        const data = await response.json();
        
        if (response.ok) {
            displayWaypoints(data);
        } else {
            showError('Failed to load waypoints');
        }
    } catch (error) {
        console.error('Error loading waypoints:', error);
        showError('Failed to connect to server');
    }
}

function displayWaypoints(waypoints) {
    const container = document.getElementById('waypointsList');
    
    if (!waypoints || waypoints.length === 0) {
        container.innerHTML = `
            <div class="empty-state">
                <i class="fas fa-map-pin"></i>
                <p>No waypoints found</p>
            </div>
        `;
        return;
    }
    
    container.innerHTML = waypoints.map(waypoint => `
        <div class="waypoint-item">
            <div class="item-header">
                <h6 class="item-title">Waypoint #${waypoint.Id}</h6>
                <div>
                    <button class="btn btn-sm btn-outline-danger" onclick="deleteWaypoint(${waypoint.Id})">
                        <i class="fas fa-trash"></i> Delete
                    </button>
                </div>
            </div>
            <div class="coordinates">
                <strong>Coordinates:</strong> ${waypoint.Latitude}, ${waypoint.Longitude}
            </div>
            <div class="mt-2">
                <small class="item-timestamp">${formatDate(waypoint.Timestamp)}</small>
                ${waypoint.Note ? `<p class="mt-1">${escapeHtml(waypoint.Note)}</p>` : ''}
            </div>
        </div>
    `).join('');
}

async function deleteWaypoint(id) {
    if (!confirm('Are you sure you want to delete this waypoint?')) {
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE}/waypoints/${id}`, {
            method: 'DELETE'
        });
        
        if (response.ok) {
            showSuccess('Waypoint deleted successfully');
            loadWaypoints();
        } else {
            const error = await response.json();
            showError(error.Error || 'Failed to delete waypoint');
        }
    } catch (error) {
        console.error('Error deleting waypoint:', error);
        showError('Failed to connect to server');
    }
}

// Notes functions
async function loadNotes() {
    try {
        showLoading('notesList');
        const response = await fetch(`${API_BASE}/notes`);
        const data = await response.json();
        
        if (response.ok) {
            displayNotes(data);
        } else {
            showError('Failed to load notes');
        }
    } catch (error) {
        console.error('Error loading notes:', error);
        showError('Failed to connect to server');
    }
}

function displayNotes(notes) {
    const container = document.getElementById('notesList');
    
    if (!notes || notes.length === 0) {
        container.innerHTML = `
            <div class="empty-state">
                <i class="fas fa-sticky-note"></i>
                <p>No notes found</p>
            </div>
        `;
        return;
    }
    
    container.innerHTML = notes.map(note => `
        <div class="note-item">
            <div class="item-header">
                <h6 class="item-title">${escapeHtml(note.Title)}</h6>
                <div>
                    <button class="btn btn-sm btn-outline-danger" onclick="deleteNote(${note.Id})">
                        <i class="fas fa-trash"></i> Delete
                    </button>
                </div>
            </div>
            <p>${escapeHtml(note.Content)}</p>
            <div class="mt-2">
                <small class="item-timestamp">${formatDate(note.Timestamp)}</small>
                ${note.Latitude && note.Longitude ? `
                    <div class="coordinates mt-1">
                        <strong>Location:</strong> ${note.Latitude}, ${note.Longitude}
                    </div>
                ` : ''}
            </div>
        </div>
    `).join('');
}

async function deleteNote(id) {
    if (!confirm('Are you sure you want to delete this note?')) {
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE}/notes/${id}`, {
            method: 'DELETE'
        });
        
        if (response.ok) {
            showSuccess('Note deleted successfully');
            loadNotes();
        } else {
            const error = await response.json();
            showError(error.Error || 'Failed to delete note');
        }
    } catch (error) {
        console.error('Error deleting note:', error);
        showError('Failed to connect to server');
    }
}

// Media functions
async function loadMedia() {
    try {
        showLoading('mediaList');
        const response = await fetch(`${API_BASE}/media`);
        const data = await response.json();
        
        if (response.ok) {
            displayMedia(data);
        } else {
            showError('Failed to load media items');
        }
    } catch (error) {
        console.error('Error loading media:', error);
        showError('Failed to connect to server');
    }
}

function displayMedia(mediaItems) {
    const container = document.getElementById('mediaList');
    
    if (!mediaItems || mediaItems.length === 0) {
        container.innerHTML = `
            <div class="empty-state">
                <i class="fas fa-images"></i>
                <p>No media items found</p>
            </div>
        `;
        return;
    }
    
    container.innerHTML = mediaItems.map(media => `
        <div class="media-item">
            <div class="item-header">
                <h6 class="item-title">${escapeHtml(media.Caption || 'Media Item')}</h6>
                <div>
                    <button class="btn btn-sm btn-outline-danger" onclick="deleteMediaItem(${media.Id})">
                        <i class="fas fa-trash"></i> Delete
                    </button>
                </div>
            </div>
            <div class="mt-2">
                <p><strong>Type:</strong> ${escapeHtml(media.Type)}</p>
                <p><strong>Path:</strong> ${escapeHtml(media.FilePath)}</p>
                ${media.ThumbnailPath ? `<p><strong>Thumbnail:</strong> ${escapeHtml(media.ThumbnailPath)}</p>` : ''}
                <small class="item-timestamp">${formatDate(media.Timestamp)}</small>
                ${media.Latitude && media.Longitude ? `
                    <div class="coordinates mt-1">
                        <strong>Location:</strong> ${media.Latitude}, ${media.Longitude}
                    </div>
                ` : ''}
            </div>
        </div>
    `).join('');
}

async function deleteMediaItem(id) {
    if (!confirm('Are you sure you want to delete this media item?')) {
        return;
    }
    
    try {
        const response = await fetch(`${API_BASE}/media/${id}`, {
            method: 'DELETE'
        });
        
        if (response.ok) {
            showSuccess('Media item deleted successfully');
            loadMedia();
        } else {
            const error = await response.json();
            showError(error.Error || 'Failed to delete media item');
        }
    } catch (error) {
        console.error('Error deleting media item:', error);
        showError('Failed to connect to server');
    }
}

// Utility functions
function showLoading(containerId) {
    const container = document.getElementById(containerId);
    if (container) {
        container.innerHTML = `
            <div class="loading-spinner">
                <div class="spinner-border text-primary" role="status">
                    <span class="visually-hidden">Loading...</span>
                </div>
            </div>
        `;
    }
}

function showError(message) {
    showToast(message, 'error');
}

function showSuccess(message) {
    showToast(message, 'success');
}

function showToast(message, type = 'info') {
    const toast = document.getElementById('toast');
    const toastBody = document.getElementById('toastBody');
    
    toastBody.textContent = message;
    
    // Update toast styling based on type
    const toastHeader = toast.querySelector('.toast-header');
    const icon = toastHeader.querySelector('i');
    
    if (type === 'error') {
        icon.className = 'fas fa-exclamation-circle me-2';
        toastHeader.style.backgroundColor = '#dc3545';
    } else if (type === 'success') {
        icon.className = 'fas fa-check-circle me-2';
        toastHeader.style.backgroundColor = '#28a745';
    } else {
        icon.className = 'fas fa-info-circle me-2';
        toastHeader.style.backgroundColor = '#007bff';
    }
    
    const bsToast = new bootstrap.Toast(toast);
    bsToast.show();
}

function updateConnectionStatus(connected) {
    const statusElement = document.getElementById('connectionStatus');
    if (statusElement) {
        statusElement.textContent = connected ? 'Connected' : 'Disconnected';
        statusElement.className = connected ? 'text-success' : 'text-danger';
    }
}

function formatDate(dateString) {
    const date = new Date(dateString);
    return date.toLocaleDateString() + ' ' + date.toLocaleTimeString();
}

function escapeHtml(text) {
    const div = document.createElement('div');
    div.textContent = text;
    return div.innerHTML;
}

function filterAdventures() {
    const searchTerm = document.getElementById('adventureSearch').value.toLowerCase();
    const adventureItems = document.querySelectorAll('#adventuresList .adventure-item');
    
    adventureItems.forEach(item => {
        const title = item.querySelector('.adventure-title').textContent.toLowerCase();
        const isVisible = title.includes(searchTerm);
        item.style.display = isVisible ? 'block' : 'none';
    });
}

function refreshAdventures() {
    loadAdventures();
}

function openCreateAdventureModal() {
    const modal = new bootstrap.Modal(document.getElementById('createAdventureModal'));
    document.getElementById('createAdventureName').value = '';
    modal.show();
}

async function createAdventure() {
    try {
        const name = (document.getElementById('createAdventureName').value || '').trim();
        if (!name) {
            showError('Please enter a name');
            return;
        }
        const response = await fetch(`${API_BASE}/adventures/create`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ name })
        });
        if (response.ok) {
            showSuccess('Adventure created');
            bootstrap.Modal.getInstance(document.getElementById('createAdventureModal')).hide();
            loadAdventures();
            loadDashboard();
        } else {
            const error = await response.json();
            showError(error.Error || 'Failed to create adventure');
        }
    } catch (err) {
        console.error('Create adventure error:', err);
        showError('Failed to connect to server');
    }
}

// Story Generator Functions
async function loadStoryAdventures() {
    try {
        showLoading('storyAdventuresList');
        const response = await fetch(`${API_BASE}/adventures`);
        const adventures = await response.json();
        
        if (response.ok) {
            displayStoryAdventures(adventures);
        } else {
            showError('Failed to load adventures for story generation');
        }
    } catch (error) {
        console.error('Error loading story adventures:', error);
        showError('Failed to connect to server');
    }
}

function displayStoryAdventures(adventures) {
    const container = document.getElementById('storyAdventuresList');
    
    if (!adventures || adventures.length === 0) {
        container.innerHTML = `
            <div class="empty-state">
                <i class="fas fa-book-open"></i>
                <p>No adventures found for story generation</p>
                <p>Create some adventures first to generate stories!</p>
            </div>
        `;
        return;
    }
    
    container.innerHTML = adventures.map(adventure => `
        <div class="adventure-item">
            <div class="adventure-header">
                <h6 class="adventure-title">${escapeHtml(adventure.Name)}</h6>
                <div>
                    <button class="btn btn-sm btn-primary me-2" onclick="generateStory(${adventure.Id})">
                        <i class="fas fa-magic me-1"></i>Generate Story
                    </button>
                    <button class="btn btn-sm btn-outline-primary" onclick="viewAdventure(${adventure.Id})">
                        <i class="fas fa-eye me-1"></i>View Details
                    </button>
                </div>
            </div>
            <div class="adventure-stats">
                <span class="stat-badge">Created: ${formatDate(adventure.CreatedAt)}</span>
                <span class="stat-badge">Waypoints: ${adventure.Waypoints?.length || 0}</span>
                <span class="stat-badge">Notes: ${adventure.Notes?.length || 0}</span>
                <span class="stat-badge">Media: ${adventure.MediaItems?.length || 0}</span>
            </div>
        </div>
    `).join('');
}

async function generateStory(adventureId) {
    try {
        showSuccess('Generating your epic adventure story...');
        window.open(`/admin/story.html?id=${adventureId}`, '_blank');
    } catch (error) {
        console.error('Error generating story:', error);
        showError('Failed to generate story');
    }
}

async function generateRandomStory() {
    try {
        showSuccess('Generating a random epic adventure story...');
        window.open('/admin/story.html', '_blank');
    } catch (error) {
        console.error('Error generating random story:', error);
        showError('Failed to generate random story');
    }
}

function openStoryGenerator() {
    window.open('/admin/story.html', '_blank');
}
