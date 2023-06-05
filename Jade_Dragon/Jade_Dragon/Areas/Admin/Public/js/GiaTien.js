function formatCurrency(input) {
    // Lấy giá trị nhập vào
    let value = input.value;
    // Xóa bỏ các dấu phẩy hiện tại
    value = value.replace(/,/g, '');
    // Chuyển đổi giá trị thành số thực
    value = parseFloat(value);
    // Kiểm tra nếu giá trị không phải là số thực, hoặc không phải là số hợp lệ
    if (isNaN(value) || value <= 0) {
        input.value = '';
    } else {
        // Định dạng giá trị thành chuỗi có dấu phẩy
        value = value.toLocaleString('en-US');
        // Gán giá trị định dạng vào ô input
        input.value = value;
    }
}