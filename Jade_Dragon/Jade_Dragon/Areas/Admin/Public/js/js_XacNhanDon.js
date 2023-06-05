const invoiceRows = document.querySelectorAll('.invoice-row');
const detailsRows = document.querySelectorAll('.details-row');
const closeButtons = document.querySelectorAll('.close-btn');
const popup = document.getElementById('popup');

// Ẩn tất cả các hàng chi tiết hóa đơn
detailsRows.forEach(row => {
    row.style.display = 'none';
});

// Bắt sự kiện click vào mã hóa đơn
invoiceRows.forEach(row => {
    row.addEventListener('click', () => {
        const invoiceId = row.getAttribute('data-invoice-id');
        const detailsRow = document.querySelector(`.details-row[data-invoice-id="${invoiceId}"]`);

        // Hiển thị/hide chi tiết hóa đơn tương ứng
        if (detailsRow.style.display === 'none') {
            detailsRow.style.display = 'table-row';
            popup.style.display = 'block';
        } else {
            detailsRow.style.display = 'none';
            popup.style.display = 'none';
        }
    });
});

// Bắt sự kiện click vào nút tắt
closeButtons.forEach(button => {
    button.addEventListener('click', () => {
        const detailsRow = button.closest('.details-row');
        const invoiceId = detailsRow.getAttribute('data-invoice-id');
        const invoiceRow = document.querySelector(`.invoice-row[data-invoice-id="${invoiceId}"]`);

        // Ẩn chi tiết hóa đơn và đóng popup
        detailsRow.style.display = 'none';
        popup.style.display = 'none';

        // Scroll đến hàng hóa đơn tương ứng
        invoiceRow.scrollIntoView();
    });
});