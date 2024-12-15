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

function isPointInTurkey(lat, lng) {
    const turkeyBounds = [
        [36.0, 26.0],
        [42.1, 45.0]
    ];
    return lat >= turkeyBounds[0][0] && lat <= turkeyBounds[1][0] &&
        lng >= turkeyBounds[0][1] && lng <= turkeyBounds[1][1];
}

map.on('click', function (e) {
    if (popupOpen) {
        map.closePopup();
        popupOpen = false;
        currentPopup = null;
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

                circleMarker.bindPopup(`
                    <div class="popup-content">
                        <h6>Ağır Metal Verisi</h6>
                        <p>Metal: ${item.metalType}</p>
                        <p>Değer: ${item.value.toFixed(6)} mg/L</p>
                        <p>Tarih: ${new Date(item.dataRecorded).toLocaleDateString('tr-TR')}</p>
                        <p>Koordinatlar: ${item.latitude.toFixed(6)}, ${item.longitude.toFixed(6)}</p>
                    </div>
                `);

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
        sidePanel.classList.toggle('open');
        body.classList.toggle('panel-open');
    });

    // Panel dışına tıklandığında paneli kapat
    document.addEventListener('click', function(event) {
        if (!sidePanel.contains(event.target) && !panelToggle.contains(event.target)) {
            sidePanel.classList.remove('open');
            body.classList.remove('panel-open');
        }
    });
});