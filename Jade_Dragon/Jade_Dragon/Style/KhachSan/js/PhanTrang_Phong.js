var currentPage = 1;
var itemsPerPage = 6;
var totalItems = document.querySelectorAll('.col').length;
var totalPages = Math.ceil(totalItems / itemsPerPage);

function showItems() {
    var items = document.querySelectorAll('.col');
    var startIndex = (currentPage - 1) * itemsPerPage;
    var endIndex = startIndex + itemsPerPage;

    for (var i = 0; i < items.length; i++) {
        if (i >= startIndex && i < endIndex) {
            items[i].style.display = 'block';
        } else {
            items[i].style.display = 'none';
        }
    }

    document.getElementById('currentPage').textContent = currentPage;
    document.getElementById('totalPages').textContent = totalPages;
}

function nextPage() {
    if (currentPage < totalPages) {
        currentPage++;
        showItems();
    }
}

function previousPage() {
    if (currentPage > 1) {
        currentPage--;
        showItems();
    }
}

showItems();