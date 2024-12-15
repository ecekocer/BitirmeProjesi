var map = L.map('map').setView([38.925533, 34.866287], 7);
const mapUrl = "https://{s}.basemaps.cartocdn.com/light_all/{z}/{x}/{y}{r}.png"
var currentCenter = map.getCenter();
var currentZoom = map.getZoom();

L.tileLayer(mapUrl, {
    maxZoom: 18,
    attribution: '&copy; <a href="https://www.opentopomap.org/">OpenTopoMap</a> contributors'
}).addTo(map);

// Panel açılıp kapandığında haritayı güncelle
document.addEventListener('DOMContentLoaded', function() {
    const panelToggle = document.querySelector('.panel-toggle');
    
    panelToggle.addEventListener('click', function() {
        setTimeout(function() {
            map.invalidateSize();
            map.setView(currentCenter, currentZoom);
        }, 300);
    });
});

// Harita hareket ettirildiğinde merkezi güncelle
map.on('moveend', function() {
    currentCenter = map.getCenter();
    currentZoom = map.getZoom();
});

var popupOpen = false;
var currentPopup = null;

// Haritadaki tüm markerları tutacak bir dizi oluşturalım
var markers = [];

var currentMarkerData = null; // Aktif marker verilerini saklamak için global değişken

function isPointInTurkey(lat, lng) {
    const turkeyBounds = [
        [36.0, 26.0],
        [42.1, 45.0]
    ];
    return lat >= turkeyBounds[0][0] && lat <= turkeyBounds[1][0] &&
        lng >= turkeyBounds[0][1] && lng <= turkeyBounds[1][1];
}

map.on('click', function (e) {
    // Eğer tıklama marker üzerinde ise, bu eventi işleme
    if (e.originalEvent.target.closest('.leaflet-marker-icon') || 
        e.originalEvent.target.closest('.leaflet-interactive')) {
        return;
    }

    // Side panel açık mı kontrol et
    const sidePanel = document.querySelector('.side-panel');
    const body = document.body;
    const isPanelOpen = sidePanel.classList.contains('open');
    
    if (isPanelOpen) {
        sidePanel.classList.remove('open');
        body.classList.remove('panel-open');
    } else if (isPointInTurkey(e.latlng.lat, e.latlng.lng)) {
        var latlng = e.latlng;
        showPopup(latlng);
    } else {
        alert('Sadece Türkiye sınırları içinde bir nokta seçebilirsiniz.');
    }
});

function showPopup(latlng) {
    window.location.href = `/PollutionEntry/Create?latitude=${latlng.lat.toFixed(6)}&longitude=${latlng.lng.toFixed(6)}`;
}

// Marker cluster group oluşturma
var markerClusterGroup = L.markerClusterGroup({
    maxClusterRadius: 80,
    iconCreateFunction: function(cluster) {
        var childCount = cluster.getChildCount();
        var size = 40;
        return L.divIcon({
            html: '<div style="width: 40px; height: 40px; border-radius: 50%; display: flex; align-items: center; justify-content: center;"><span>' + childCount + '</span></div>',
            className: 'marker-cluster',
            iconSize: new L.Point(size, size)
        });
    }
});

map.addLayer(markerClusterGroup);

// Global fetchLocationData function
function fetchLocationData(latitude, longitude, filters = {}) {
    const queryParams = new URLSearchParams({
        latitude: latitude,
        longitude: longitude,
        ...filters
    });

    fetch(`/PollutionEntry/GetPollutionDataByCoordinates?${queryParams}`)
        .then(response => response.json())
        .then(result => {
            if (result.success) {
                // Create the HTML content for the records
                const recordsHtml = result.data.length > 0 ? result.data.map(record => `
                    <div class="pollution-record mb-3 p-3 bg-white rounded shadow-sm">
                        <div class="d-flex justify-content-between">
                            <strong>Yıl:</strong> ${record.year}
                        </div>
                        <div class="d-flex justify-content-between">
                            <strong>Metal Türü:</strong> ${record.metalType}
                        </div>
                        <div class="d-flex justify-content-between">
                            <strong>Değer:</strong> ${record.value}
                        </div>
                    </div>
                `).join('') : '<div class="text-white">Bu konumda filtrelere uygun veri bulunamadı.</div>';

                // Update records panel content
                const recordsPanel = document.getElementById('recordsPanel');
                recordsPanel.innerHTML = `
                    <h5 class="text-white mb-3">Konum Kayıtları</h5>
                    <div class="records-container">
                        ${recordsHtml}
                    </div>
                `;

                // Update add button click handler
                const addDataButton = document.getElementById('addDataButton');
                addDataButton.onclick = () => addNewData(latitude, longitude);
            }
        });
}

function addNewData(latitude, longitude) {
    window.location.href = `/PollutionEntry/Create?latitude=${latitude}&longitude=${longitude}`;
}

function resetFilters() {
    const metalTypeSelect = document.getElementById('metalType');
    const yearSelect = document.getElementById('year');
    
    // Filtreleri sıfırla
    metalTypeSelect.value = '';
    yearSelect.value = '';

    // Eğer aktif bir marker varsa, verilerini yeniden yükle
    if (currentMarkerData) {
        fetchLocationData(currentMarkerData.latitude, currentMarkerData.longitude);
    }
}

// Veritabanından verileri çekip marker'ları oluştur
function loadPollutionData() {
    fetch('/Home/GetPollutionData')
        .then(response => response.json())
        .then(data => {
            data.forEach(item => {
                var circleMarker = L.circleMarker([item.latitude, item.longitude], {
                    radius: 8,
                    fillColor: '#000080',  // Koyu mavi
                    color: '#000080',
                    weight: 1,
                    opacity: 1,
                    fillOpacity: 0.4
                });

                circleMarker.on('click', function(e) {
                    const sidePanel = document.querySelector('.side-panel');
                    const body = document.body;
                    const isPanelOpen = sidePanel.classList.contains('open');

                    // Aktif marker verilerini sakla
                    currentMarkerData = item;

                    // Initial data fetch
                    fetchLocationData(item.latitude, item.longitude);

                    // Add event listeners to filter elements
                    const metalTypeSelect = document.getElementById('metalType');
                    const yearSelect = document.getElementById('year');

                    function applyLocationFilters() {
                        const filters = {};
                        
                        // Sadece seçili değerleri filtrelere ekle
                        if (metalTypeSelect.value) {
                            filters.metalType = metalTypeSelect.value;
                        }
                        
                        if (yearSelect.value) {
                            filters.year = yearSelect.value;
                        }

                        fetchLocationData(item.latitude, item.longitude, filters);
                    }

                    // Add event listeners
                    metalTypeSelect.addEventListener('change', applyLocationFilters);
                    yearSelect.addEventListener('change', applyLocationFilters);

                    if (!isPanelOpen) {
                        sidePanel.classList.add('open');
                        body.classList.add('panel-open');
                    }
                    
                    // Event'in harita click event'ine ulaşmasını engelle
                    L.DomEvent.stopPropagation(e);
                });

                markerClusterGroup.addLayer(circleMarker);
            });
        });
}

// Sayfa yüklendiğinde verileri yükle
loadPollutionData();

document.addEventListener('DOMContentLoaded', function() {
    const panelToggle = document.querySelector('.panel-toggle');
    const sidePanel = document.querySelector('.side-panel');
    const body = document.body;

    panelToggle.addEventListener('click', function() {
        const isPanelOpen = sidePanel.classList.contains('open');
        if (isPanelOpen) {
            sidePanel.classList.remove('open');
            body.classList.remove('panel-open');
        } else {
            sidePanel.classList.add('open');
            body.classList.add('panel-open');
        }
    });

    // Panel dışına tıklandığında paneli kapat
    document.addEventListener('click', function(event) {
        // Marker veya cluster'a tıklandığında paneli kapatma
        if (event.target.closest('.leaflet-marker-icon') || 
            event.target.closest('.leaflet-interactive')) {
            return;
        }
        
        if (!sidePanel.contains(event.target) && !panelToggle.contains(event.target)) {
            sidePanel.classList.remove('open');
            body.classList.remove('panel-open');
        }
    });
});