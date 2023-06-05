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

    if ('geolocation' in navigator) {
        navigator.geolocation.getCurrentPosition(function (position) {
            // thêm giá trị kinh độ và vĩ độ vào input
            var dv_kinhdo = document.getElementById("Dv_KinhDo");
            var dv_vido = document.getElementById("Dv_ViDo");
            dv_kinhdo.value = position.coords.longitude;
            dv_vido.value = position.coords.latitude;
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
                color: 'black'
            }),
            stroke: new ol.style.Stroke({
                color: 'black',
                width: 5
            })
        })
    }));

    // Xử lý thông tin địa điểm khi click vào bản đồ
    map.on('singleclick', function (evt) {
        var maks = $('#maks_map').val();
        if (maks == null || maks.trim() == '') {
            $('#DangKy_Map').css('z-index', '-999');
            // lấy tọa độ vị trí click
            var clickedCoordinate = evt.coordinate;
            var lonlat = ol.proj.toLonLat(clickedCoordinate);
            // Di chuyển marker tới vị trí click
            marker.getGeometry().setCoordinates(clickedCoordinate);

            // lấy kinh độ và vĩ độ từ tọa độ click
            var kinhdo = lonlat[0];
            var vido = lonlat[1];

            // thêm giá trị kinh độ và vĩ độ vào input
            var dv_kinhdo = document.getElementById("Dv_KinhDo");
            var dv_vido = document.getElementById("Dv_ViDo");
            dv_kinhdo.value = kinhdo;
            dv_vido.value = vido;
        } else {
            handlePosition();
        }
    });

    function handlePosition() {
        var Dv_KinhDo = $('#Dv_KinhDo').val();
        var Dv_ViDo = $('#Dv_ViDo').val();

        var maks = $('#maks_map').val();
        var tenks = $('#tenks_map').val();
        var sdt = $('#sdtks_map').val();
        var gmail = $('#gmailks_map').val();
        var diachi = $('#diachiks_map').val();
        var gia = $('#giaks_map').val();
        var anhks = $('#avtks_map').val();
        var KinhDo = $('#kinhdo_map').val();
        var ViDo = $('#vido_map').val();

        // Hiển thị thông tin lên bản đồ
        var div_form = document.getElementById("DangKy_Map");
        var info = document.getElementById("info_ksks");
        if (info) {
            info.style.background = "rgba(0, 0, 0, 0.7)";
            div_form.style.zIndex = "999";
            if (anhks.length > 0) {
                info.innerHTML = `
                                <button id="thoat_form"><img src="/Style/img/icon/icon-X.jpg" style="width: 10px;" title="Đóng"/></button>
                                <h2>Khách sạn ${tenks}</h2>
                                <div class="anhavtks">
                                    <img src="/UpLoad_Img/KhachSan/${anhks}" alt="">
                                </div>
                                <p class="dc">Địa chỉ: ${diachi}</p>
                                <h2>${gia} VND</h2>
                                <div style="display: flex;">
                                    <a href="/khachsan/khachsan?ma= ${maks}" class="oke">Đặt Phòng</a>
                                    <button id="btnChiduong">Chỉ đường</button>
                                </div>
                                <div class="lienhe_map">
                                    <p>Gmail: <br/> ${gmail}</p>
                                    <p>Sđt: <br/> ${sdt}</p>
                                </div>`;
                $('#btnChiduong').click(function () {
                    var ChiDuong = [
                        {
                            DiemDi: [Dv_ViDo, Dv_KinhDo],
                            DiemDen: [ViDo, KinhDo]
                        },
                    ];
                    SaveInfo(ChiDuong);
                });
                $('#thoat_form').click(function () {
                    $('#DangKy_Map').css('z-index', '-999');
                });
            } else {
                info.innerHTML = `
                                <h2>Khách sạn ${tenks}</h2>
                                <div class="anhavtks">
                                    <p>Ảnh hiện chưa có !</p>
                                </div>
                                <p class="dc">Địa chỉ: ${diachi}</p>
                                <h2>${gia} VND</h2>
                                <div style="display: flex;">
                                    <a href="/khachsan/khachsan?ma= ${maks}" class="oke">Đặt Phòng</a>
                                    <button id="btnChiduong">Chỉ đường</button>
                                </div>
                                <div class="lienhe_map">
                                    <p>Gmail: <br/> ${gmail}</p>
                                    <p>Sđt: <br/> ${sdt}</p>
                                </div>`;
                $('#btnChiduong').click(function () {
                    var ChiDuong = [
                        {
                            DiemDi: [Dv_ViDo, Dv_KinhDo],
                            DiemDen: [ViDo, KinhDo]
                        },
                    ];
                    SaveInfo(ChiDuong);
                });
                $('#thoat_form').click(function () {
                    $('#DangKy_Map').css('z-index', '-999');
                });
            }
        }
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
        var avtks = location.avt;

        // Tạo feature của marker với nội dung thông tin khách sạn tương ứng
        var marker = new ol.Feature({
            geometry: new ol.geom.Point(ol.proj.fromLonLat([lon, lat])),
            content:
                `<div style="background-color: black; color: wheat; width: 400px; word-wrap: break-word; padding: 10px; border: 2px solid yellow; cursor: pointer;">` +
                `<input type="hidden" id="maks_map" value="` + maks_map + `"/>` +
                '<div><strong> Khách Sạn: ' + name + '</strong>' +
                `<input type="hidden" id="tenks_map" value="` + name + `"/></div>` +
                '<div>Địa chỉ: ' + address + '</div>' +
                `<input type="hidden" id="diachiks_map" value="` + address + `"/>` +
                '<div>Số điện thoại: ' + phone + '</div>' +
                `<input type="hidden" id="sdtks_map" value="` + phone + `"/>` +
                '<div >Gmail: ' + gmail + '</div>' +
                `<input type="hidden" id="gmailks_map" value="` + gmail + `"/>` +
                '<div >Giá tiền: ' + moeny + '</div>' +
                `<input type="hidden" id="giaks_map" value="` + moeny + `"/>` +
                `<input type="hidden" id="avtks_map" value="` + avtks + `"/></div>` +
                `<input type="hidden" id="kinhdo_map" value="` + lon + `"/></div>` +
                `<input type="hidden" id="vido_map" value="` + lat + `"/></div>`,
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

    function SaveInfo(ChiDuong) {
        // Tạo lớp chỉ đường mới
        var directionsLayer = new ol.layer.Vector({
            name: 'directionsLayer',
            source: new ol.source.Vector(),
            style: new ol.style.Style({
                stroke: new ol.style.Stroke({
                    color: '#2c3e50',
                    width: 5
                })
            })
        });

        ChiDuong.forEach(function (haha) {
            getDirections(haha, directionsLayer);
        });

        // Xóa lớp chỉ đường cũ (nếu có)
        map.getLayers().forEach(function (layer) {
            if (layer.get('name') === 'directionsLayer') {
                map.removeLayer(layer);
            }
        });

        // Thêm lớp chỉ đường mới vào bản đồ
        map.addLayer(directionsLayer);
    }

    // Hàm để lấy các chỉ dẫn từ vị trí hiện tại đến vị trí đích
    function getDirections(ChiDuong, directionsLayer) {
        const ORS_API_KEY = '5b3ce3597851110001cf62489fa5985f04e04c5e9fcc32a984e681cf';

        const DiemDi = ChiDuong.DiemDi;
        const DiemDen = ChiDuong.DiemDen;

        const url = `https://api.openrouteservice.org/v2/directions/driving-car?api_key=${ORS_API_KEY}&start=${DiemDi[1]},${DiemDi[0]}&end=${DiemDen[1]},${DiemDen[0]}`;
        const k = url;
        fetch(url)
            .then(response => response.json())
            .then(data => {
                const route = data.features[0].geometry.coordinates;
                const lineString = new ol.geom.LineString(route).transform('EPSG:4326', 'EPSG:3857');
                const feature = new ol.Feature({
                    geometry: lineString
                });

                // Thêm chỉ dẫn vào lớp chỉ đường mới
                directionsLayer.getSource().addFeature(feature);
            })
            .catch(error => {
                console.error('Lỗi: ', error);
            });
    }

    return map;
}