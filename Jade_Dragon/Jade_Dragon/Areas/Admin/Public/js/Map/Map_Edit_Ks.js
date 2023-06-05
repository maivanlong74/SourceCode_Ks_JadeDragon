var map;
function initMap(hotels) {
    var longitude = hotels[0].coordinates[0];
    var latitude = hotels[0].coordinates[1];

    var view = new ol.View({
        center: ol.proj.fromLonLat([longitude, latitude]),
        zoom: 17
    });

    map = new ol.Map({
        target: 'map',
        layers: [
            new ol.layer.Tile({
                source: new ol.source.OSM()
            })
        ],
        view: view
    });

    // Xử lý thông tin địa điểm khi click vào bản đồ
    map.on('singleclick', function (evt) {
        var maks = $('#maks_map').val();

        if (maks == null || maks.trim() == '') {
            var clickedCoordinate = evt.coordinate;
            var lonlat = ol.proj.toLonLat(clickedCoordinate);
            // Di chuyển marker tới vị trí click
            marker.getGeometry().setCoordinates(clickedCoordinate);
            handlePosition(lonlat);
        } else {
            window.location.href = "~/khachsan/khachsan?ma=" + maks;
        }

    });

    function handlePosition(lonlat) {
        // Tìm kiếm địa điểm gần vị trí chọn nhất và hiển thị thông tin lên bản đồ
        var url = `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lonlat[1]}&lon=${lonlat[0]}&addressdetails=1&zoom=18`;
        fetch(url)
            .then(response => response.json())
            .then(data => {
                console.log(data);
                var address = data.address;
                var houseNumber = address.house_number || '';
                var road = address.road || '';
                var suburb = address.suburb || address.neighbourhood || '';
                var city = address.city || address.town || address.village || '';
                var state = address.state || '';
                var displayName = `${houseNumber} ${road}, ${suburb}, ${city}, ${state}`;

                var latitude = lonlat[1];
                var longitude = lonlat[0];

                $('#DiaChi').val(displayName);
                $('#KhuVuc').val(road);
                $('input[name="KinhDo"]').val(longitude);
                $('input[name="ViDo"]').val(latitude);

                var MuiTen = document.querySelector('.MuiTen');
                var mapElement = document.getElementById("body");
                MuiTen.classList.remove("danhmucsp");
                mapElement.classList.add("danhmucsp");
                var danhmucsp = document.querySelector('.danhmucsp');
                if (danhmucsp) {
                    danhmucsp.scrollIntoView({ behavior: 'smooth' });
                }
            })
            .catch(error => {
                console.error("Lỗi khi tìm nạp dữ liệu:", error);
            });
    }

    // Thêm control zoom slider
    var zoomSlider = new ol.control.ZoomSlider();
    map.addControl(zoomSlider);
    // Thêm control full screen
    var fullScreen = new ol.control.FullScreen();
    map.addControl(fullScreen);
    // Thêm control OverviewMap
    var OverviewMap = new ol.control.OverviewMap();
    map.addControl(OverviewMap);
    // Thêm control ScaleLine
    var ScaleLine = new ol.control.ScaleLine();
    map.addControl(ScaleLine);

    /*--------*/
    // Lấy các phần tử input và button tìm kiếm
    var searchInput = document.getElementById('search-input');
    var searchButton = document.getElementById('search-button');

    // Xử lý sự kiện click vào nút tìm kiếm
    searchButton.addEventListener('click', function () {
        var searchText = searchInput.value;
        if (searchText !== '') {
            // Kiểm tra xem người dùng đã nhập vào một từ khóa hay là một tọa độ
            if (isNaN(searchText)) {
                // Tìm kiếm địa điểm dựa trên từ khóa
                searchPlaceByName(searchText);
            } else {
                // Chuyển đổi tọa độ kinh độ/vĩ độ thành đối tượng OpenLayers và di chuyển marker tới vị trí đó
                var coordinates = [parseFloat(searchText.split(',')[0]), parseFloat(searchText.split(',')[1])];
                var pos = ol.proj.fromLonLat(coordinates);
                marker.getGeometry().setCoordinates(pos);
                // Set lại center của map
                map.getView().setCenter(pos);
                map.getView().setZoom(15);
            }
        }
    });

    // Xử lý sự kiện nhấn phím Enter để gửi tin nhắn
    $('#search-input').keypress(function (e) {
        if (e.which === 13) { // Kiểm tra xem phím Enter đã được bấm chưa
            var searchText = searchInput.value;
            if (searchText !== '') {
                // Kiểm tra xem người dùng đã nhập vào một từ khóa hay là một tọa độ
                if (isNaN(searchText)) {
                    // Tìm kiếm địa điểm dựa trên từ khóa
                    searchPlaceByName(searchText);
                } else {
                    // Chuyển đổi tọa độ kinh độ/vĩ độ thành đối tượng OpenLayers và di chuyển marker tới vị trí đó
                    var coordinates = [parseFloat(searchText.split(',')[0]), parseFloat(searchText.split(',')[1])];
                    var pos = ol.proj.fromLonLat(coordinates);
                    marker.getGeometry().setCoordinates(pos);
                    // Set lại center của map
                    map.getView().setCenter(pos);
                    map.getView().setZoom(15);
                }
            }
        }
    });

    // Hàm tìm kiếm địa điểm dựa trên từ khóa
    function searchPlaceByName(searchText) {
        var url = `https://nominatim.openstreetmap.org/search?q=${searchText}&format=json`;
        fetch(url)
            .then(response => response.json())
            .then(data => {
                if (data.length > 0) {
                    var result = data[0];
                    var pos = ol.proj.fromLonLat([parseFloat(result.lon), parseFloat(result.lat)]);
                    // Di chuyển marker tới vị trí tìm kiếm được
                    marker.getGeometry().setCoordinates(pos);

                    // Set lại center của map
                    map.getView().setCenter(pos);
                    map.getView().setZoom(18);

                    var lonlat = ol.proj.toLonLat(pos);
                    handlePosition(lonlat);
                } else {
                    alert('Không tìm thấy địa điểm');
                }
            })
    }

    // Tạo một đối tượng marker
    var marker = new ol.Feature({
        geometry: new ol.geom.Point(ol.proj.fromLonLat([0, 0]))
    });

    marker.setStyle(new ol.style.Style({
        image: new ol.style.Icon({
            anchor: [0.5, 1],
            src: 'https://openlayers.org/en/latest/examples/data/icon.png'
        })
    }));

    // Tạo một lớp vector để chứa marker
    var vectorLayer = new ol.layer.Vector({
        source: new ol.source.Vector({
            features: [marker]
        })
    });
    // Thêm lớp vector vào bản đồ
    map.addLayer(vectorLayer);

    // Tạo các lớp vector và thêm chúng vào bản đồ
    hotels.forEach(function (lonlat) {
        var vectorLayer = Search_KD_VD(lonlat);
        map.addLayer(vectorLayer);
    });

    var overlays = [];
    var maks_map = null;

    function Search_KD_VD(location) {
        var lon = location.coordinates[0];
        var lat = location.coordinates[1];
        maks_map = location.maks || null;
        var name = location.name;
        var address = location.address;
        var phone = location.phone;
        var gmail = location.gmail;
        var moeny = location.moeny;

        // Tạo feature của marker với nội dung thông tin khách sạn tương ứng
        var marker = new ol.Feature({
            geometry: new ol.geom.Point(ol.proj.fromLonLat([lon, lat])),
            content:
                `<div style="background-color: black; color: wheat; width: 400px; word-wrap: break-word; padding: 10px; border: 2px solid yellow;">` +
                `<input type="hidden" id="maks_map" value="` + maks_map + `"/>` +
                '<div><strong> Khách Sạn: ' + name + '</strong></div>' +
                '<div>Địa chỉ: ' + address + '</div>' +
                '<div>Số điện thoại: ' + phone + '</div>' +
                '<div>Gmail: ' + gmail + '</div>' +
                '<div>Giá tiền: ' + moeny + '</div>' +
                `</div>`,
            maks: maks_map
        });
        marker.setStyle(new ol.style.Style({
            image: new ol.style.Icon({
                anchor: [0.5, 1],
                src: 'https://openlayers.org/en/latest/examples/data/icon.png'
            })
        }));

        var vectorLayer = new ol.layer.Vector({
            source: new ol.source.Vector({
                features: [marker]
            })
        });

        // Tạo một interaction để chọn feature
        var selectInteraction = new ol.interaction.Select({
            layers: [vectorLayer],
        });

        // Thêm interaction vào map
        map.addInteraction(selectInteraction);

        // Tạo overlay cho feature marker để hiển thị thông tin khách sạn khi hover chuột vào
        var popup = new ol.Overlay({
            element: document.createElement('div'),
            autoPan: false,
            autoPanAnimation: {
                duration: 250
            },
            positioning: "bottom-center",
            stopEvent: false,
            offset: [0, -50],
        });
        map.addOverlay(popup);

        map.on('pointermove', function (evt) {
            var feature = map.forEachFeatureAtPixel(evt.pixel,
                function (feature, layer) {
                    return feature;
                });
            if (feature) {
                var coordinates = feature.getGeometry().getCoordinates();
                var maks_map = feature.get('maks') || null;
                if (maks_map) {
                    popup.getElement().innerHTML = `<input type="hidden" id="maks_map" value="${maks_map}"/>` + feature.get('content');
                } else {
                    popup.getElement().innerHTML = feature.get('content');
                }
                popup.setPosition(coordinates);
                popup.getElement().style.display = 'block';
            } else {
                $('#maks_map').val(null);
                popup.getElement().style.display = 'none';
            }
        });

        return vectorLayer;
    }

    var popup = new ol.Overlay({
        element: document.createElement('div'),
        autoPan: false,
        autoPanAnimation: {
            duration: 250
        },
        positioning: "bottom-center",
        stopEvent: false,
        offset: [0, -50],
        id: "popup" // thêm id vào overlay
    });

    map.on('pointermove', function (evt) {
        var feature = map.forEachFeatureAtPixel(evt.pixel,
            function (feature, layer) {
                return feature;
            });
        if (feature) {
            var coordinates = feature.getGeometry().getCoordinates();
            popup.setPosition(coordinates);
            var content = feature.get('content');
            document.getElementById('popup').innerHTML = content;
            map.getViewport().style.cursor = 'pointer';
        } else {
            popup.setPosition(undefined);
            map.getViewport().style.cursor = '';
        }
    });

    map.on('pointerout', function () {
        popup.setPosition(undefined);
        map.getViewport().style.cursor = '';
    });
    return map;
}

$('#MuiTen').click(function () {
    var map = document.getElementById("map");
    var body = document.getElementById("body");
    var muiten1 = document.getElementById("muiten_map1");
    var muiten2 = document.getElementById("muiten_map2");
    body.classList.remove("danhmucsp");
    map.classList.add("danhmucsp");
    muiten1.style.display = "none";
    muiten2.style.display = "block";

    var danhmucsp = document.querySelector('.danhmucsp');
    if (danhmucsp) {
        danhmucsp.scrollIntoView({ behavior: 'smooth' });
    }
});

$('#MuiTen_Top').click(function () {
    var map = document.getElementById("map");
    var body = document.getElementById("body");
    var muiten1 = document.getElementById("muiten_map1");
    var muiten2 = document.getElementById("muiten_map2");
    map.classList.remove("danhmucsp");
    body.classList.add("danhmucsp");
    muiten2.style.display = "none";
    muiten1.style.display = "block";

    var danhmucsp = document.querySelector('.danhmucsp');
    if (danhmucsp) {
        danhmucsp.scrollIntoView({ behavior: 'smooth' });
    }
});