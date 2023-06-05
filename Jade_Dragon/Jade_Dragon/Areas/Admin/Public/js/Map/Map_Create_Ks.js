var map;
function initMap(hotels) {
    map = new ol.Map({
        target: 'map',
        layers: [
            new ol.layer.Tile({
                source: new ol.source.OSM()
            })
        ],
    });
    // ------------------------------
    if ('geolocation' in navigator) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var pos = ol.proj.fromLonLat([position.coords.longitude, position.coords.latitude]);
            var data = {
                latitude: position.coords.latitude,
                longitude: position.coords.longitude
            };
            var marker = new ol.Feature({
                geometry: new ol.geom.Point(pos)
            });
            var markerSource = new ol.source.Vector({
                features: [marker]
            });
            var markerLayer = new ol.layer.Vector({
                source: markerSource
            });
            map.addLayer(markerLayer);
            map.getView().setCenter(pos);
            map.getView().setZoom(15);
        });
    } else {
        alert('Định vị địa lý không được trình duyệt của bạn hỗ trợ');
    }

    // Thêm control để xác định vị trí hiện tại
    var geolocation = new ol.Geolocation({
        trackingOptions: {
            enableHighAccuracy: true
        },
        projection: map.getView().getProjection()
    });
    var positionFeature = new ol.Feature();
    positionFeature.setStyle(new ol.style.Style({
        image: new ol.style.Circle({
            radius: 6,
            fill: new ol.style.Fill({
                color: '#3399CC'
            }),
            stroke: new ol.style.Stroke({
                color: '#fff',
                width: 2
            })
        })
    }));

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
        var url = `https://nominatim.openstreetmap.org/reverse?format=jsonv2&lat=${lonlat[1]}&lon=${lonlat[0]}&addressdetails=1&zoom=18`;
        fetch(url)
            .then(response => response.json())
            .then(data => {
                var address = data.address;
                var houseNumber = address.house_number || '';
                var road = address.road || '';
                var suburb = address.suburb || address.neighbourhood || '';
                var city = address.city || address.town || address.village || '';
                var state = address.state || '';
                var displayName = `${houseNumber} ${road}, ${suburb}, ${city}, ${state}`;

                var latitude = lonlat[1];
                var longitude = lonlat[0];

                // Hiển thị thông tin lên bản đồ
                var div_form = document.getElementById("DangKy_Map");
                var formdk = document.getElementById("register-form");
                if (formdk) {
                    formdk.style.background = "rgba(0, 0, 0, 0.7)";
                    div_form.style.zIndex = "999";
                    formdk.innerHTML = `
                                <label for="name">Tên khách sạn:</label>
                                <input style="margin-left: 10px;" type="text" id="TenKhachSan" name="TenKhachSan" required> <br/>

                                <label for="phone">Số điện thoại:</label>
                                <input style="margin-left: 24px;" type="tel" id="SoDienThoai" name="SoDienThoai" required> <br/>

                                <label for="email">Email:</label>
                                <input style="margin-left: 80px;" type="email" id="Gmail" name="Gmail" required> <br/>

                                <label for="address">Địa chỉ:</label>
                                <input style="margin-left: 70px;" type="text" id="DiaChi" name="DiaChi" value="${displayName}" required> <br/>

                                <label for="price">Giá:</label>
                                <input style="margin-left: 99px;" type="text" id="Gia" name="GiaTien" oninput="formatCurrency(this)" required> <br/>

                                <label for="image">Ảnh:</label>
                                <input style="margin-left: 140px; width: 78px;" 
                                    type="file" id="Avt" name="Avt" accept="image/*"> <br/>
                                <input type="hidden" id="KhuVuc" name="TenKhuVuc" value="${suburb}">

                                <input type="hidden" id="KinhDo" name="KinhDo" value="${longitude}">
                                <input type="hidden" id="ViDo" name="ViDo" value="${latitude}">
                                <button type="submit">Đăng ký</button>`;
                } else {
                    console.log("Không tìm thấy phần tử có ID 'kết quả'");
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
                    // Tìm khách sạn theo tên
                    var result = _.find(hotels, { name: searchText });
                    var k = result;
                    if (result) {
                        var pos = ol.proj.fromLonLat(result.coordinates);
                        marker.getGeometry().setCoordinates(pos);
                        map.getView().setCenter(pos);
                        map.getView().setZoom(18);
                    } else {
                        var resultt = data[0];
                        var pos = ol.proj.fromLonLat([parseFloat(resultt.lon), parseFloat(resultt.lat)]);
                        // Di chuyển marker tới vị trí tìm kiếm được
                        marker.getGeometry().setCoordinates(pos);
                        // Set lại center của map
                        map.getView().setCenter(pos);
                        map.getView().setZoom(18);
                    }
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
            src: '/Style/img/icon/icon-dinhvi.jpg'
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
                `<div style="background-color: black; color: wheat; width: 400px; word-wrap: break-word; padding: 10px; border: 2px solid yellow; cursor: pointer;">` +
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