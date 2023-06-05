window.addEventListener('load', function () {
    var keotruot = document.getElementsByClassName('keotruot')[0];
    keotruot.scrollIntoView({ behavior: 'smooth' });
});

$('#XemMap').click(function () {
    var body = document.getElementById("htkhachsan");
    var mapElement = document.getElementById("map");

    body.classList.remove("keotruot");
    mapElement.classList.add("keotruot");
    var keotruot = document.querySelector('.keotruot');
    if (keotruot) {
        keotruot.scrollIntoView({ behavior: 'smooth' });
    }
});

$('#XemKS').click(function () {
    var body = document.getElementById("htkhachsan");
    var mapElement = document.getElementById("map");

    mapElement.classList.remove("keotruot");
    body.classList.add("keotruot");
    var keotruot = document.querySelector('.keotruot');
    if (keotruot) {
        keotruot.scrollIntoView({ behavior: 'smooth' });
    }
});

$('#Open_Map').click(function () {
    var info_ks = document.getElementById("info_ks");
    var mapElement = document.getElementById("map");

    info_ks.classList.remove("keotruot");
    mapElement.classList.add("keotruot");
    mapElement.style.zIndex = "999";
    var keotruot = document.querySelector('.keotruot');
    if (keotruot) {
        keotruot.scrollIntoView({ behavior: 'smooth' });
    }
});

$('#thoat_map').click(function () {
    var info_ks = document.getElementById("info_ks");
    var mapElement = document.getElementById("map");

    mapElement.classList.remove("keotruot");
    info_ks.classList.add("keotruot");
    mapElement.style.zIndex = "-999";
    var keotruot = document.querySelector('.keotruot');
    if (keotruot) {
        keotruot.scrollIntoView({ behavior: 'smooth' });
    }
});