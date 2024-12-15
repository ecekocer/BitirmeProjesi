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
        // Tüm cluster'lar için aynı boyut ve renk kullanılacak
        var size = childCount * 15;
        var color = '#000080'; // Koyu mavi
        var circleSize = childCount * 15;

        if(childCount > 10){
            circleSize = 100;
            size = 100;
        }

        return L.divIcon({
            html: '<div style="background-color: rgba(0, 0, 128, 0.4); border-radius: 50%; width: ' + circleSize + 'px; height: ' + circleSize + 'px;"></div>',
            className: 'marker-cluster',
            iconSize: new L.Point(size, size)
        });
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
                    <div class="pollution-record">
                        <div class="record-content">
                            <div class="record-row">
                                <div class="record-item">
                                    <i class="fas fa-calendar-alt"></i>
                                    <span class="item-value">${record.year}</span>
                                </div>
                                <div class="record-item">
                                    <i class="fas fa-flask"></i>
                                    <span class="item-value">${record.metalType}</span>
                                </div>
                                <div class="record-item">
                                    <i class="fas fa-chart-line"></i>
                                    <span class="item-value">${record.value.toFixed(2)}</span>
                                </div>
                            </div>
                        </div>
                    </div>
                `).join('') : '<div class="no-data"><i class="fas fa-info-circle"></i> Bu konumda filtrelere uygun veri bulunamadı.</div>';

                // Update records panel content
                const recordsPanel = document.getElementById('recordsPanel');
                recordsPanel.innerHTML = `
                    <div class="records-header">
                        <h5 class="text-white mb-2">
                            <i class="fas fa-map-marker-alt me-2"></i>
                            Konum Kayıtları
                        </h5>
                        <div class="records-count text-white-50">
                            ${result.data.length} kayıt
                        </div>
                    </div>
                    <div class="records-container">
                        <div class="records-column-headers">
                            <div class="column-header">
                                <i class="fas fa-calendar-alt"></i>
                                <span>Yıl</span>
                            </div>
                            <div class="column-header">
                                <i class="fas fa-flask"></i>
                                <span>Metal Türü</span>
                            </div>
                            <div class="column-header">
                                <i class="fas fa-chart-line"></i>
                                <span>Değer</span>
                            </div>
                        </div>
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
                var {fillColor, color} = getMetalTypeColors(item.metalType)
                var circleMarker = L.circleMarker([item.latitude, item.longitude], {
                    radius: 8,
                    fillColor: fillColor,  // Koyu mavi
                    color: color,
                    weight: 1,
                    opacity: 1,
                    fillOpacity: 1
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

function getMetalTypeColors(metalType) {
    switch(metalType) {
        case "Arsenik":
            return {
                fillColor: "#FF4C4C", // Kırmızı
                color: "#8B0000"      // Koyu kırmızı
            };
        case "Kadmiyum":
            return {
                fillColor: "#FFA500", // Turuncu
                color: "#8B4513"      // Kahverengi
            };
        case "Krom":
            return {
                fillColor: "#4CAF50", // Yeşil
                color: "#1B5E20"      // Koyu yeşil
            };
        case "Bakır":
            return {
                fillColor: "#CD7F32", // Bronz
                color: "#8B4513"      // Kahverengi
            };
        case "Civa":
            return {
                fillColor: "#C0C0C0", // Gümüş
                color: "#696969"      // Koyu gri
            };
        case "Nikel":
            return {
                fillColor: "#B8860B", // Altın sarısı
                color: "#8B6914"      // Koyu altın
            };
        case "Kurşun":
            return {
                fillColor: "#778899", // Kurşuni
                color: "#2F4F4F"      // Koyu kurşuni
            };
        case "Çinko":
            return {
                fillColor: "#87CEEB", // Açık mavi
                color: "#4682B4"      // Çelik mavisi
            };
        default:
            return {
                fillColor: "#808080", // Gri
                color: "#404040"      // Koyu gri
            };
    }
}