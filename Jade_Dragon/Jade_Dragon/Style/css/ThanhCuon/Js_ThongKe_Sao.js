// Đánh giá độ phổ biến của từng sao (ví dụ)
const ratings = [20, 30, 50, 10, 100];

// Lấy các phần tử giao diện
const starElements = document.querySelectorAll('.starr');
const ratingCountElement = document.querySelector('.rating-count');
const demElements = document.querySelectorAll('.dem');

// Tính tổng rating và tỷ lệ điều chỉnh
const totalRatings = ratings.reduce((total, rating) => total + rating, 0);
const adjustmentRatio = 100 / totalRatings;

// Cập nhật số sao và độ phổ biến
starElements.forEach((starr, index) => {
    const adjustedWidth = ratings[index] * adjustmentRatio;
    starr.style.width = adjustedWidth + '%';
    demElements[index].textContent = ratings[index];
});

ratingCountElement.textContent = totalRatings;