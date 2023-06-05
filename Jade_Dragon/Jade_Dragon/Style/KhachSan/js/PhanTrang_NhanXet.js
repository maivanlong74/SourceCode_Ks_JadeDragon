var currentPage = 1;
var itemsPerPage = 6;
var totalItems = document.querySelectorAll('.ht-cmt-item').length;
var totalPages = Math.ceil(totalItems / itemsPerPage);

function showItems() {
    var items = document.querySelectorAll('.ht-cmt-item');
    var startIndex = (currentPage - 1) * itemsPerPage;
    var endIndex = startIndex + itemsPerPage;

    for (var i = 0; i < items.length; i++) {
        if (i >= startIndex && i < endIndex) {
            items[i].style.display = 'block';
        } else {
            items[i].style.display = 'none';
        }
    }

    document.getElementById('TrangSau').textContent = currentPage;
    document.getElementById('TrangTruoc').textContent = totalPages;
}

function TienTrang() {
    if (currentPage < totalPages) {
        currentPage++;
        showItems();
    }
}

function LuiTrang() {
    if (currentPage > 1) {
        currentPage--;
        showItems();
    }
}
showItems();