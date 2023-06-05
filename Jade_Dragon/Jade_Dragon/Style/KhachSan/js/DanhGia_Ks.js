const ratings = document.querySelectorAll('.rating');
ratings.forEach(rating => {
    const stars = rating.querySelectorAll('.star');
    const ratingValue = rating.querySelector('.rating-value');
    const initialRating = parseInt(ratingValue.value);
    let hasConfirmed = false; // Trạng thái xác nhận

    if (!isNaN(initialRating) && initialRating >= 1 && initialRating <= 5) {
        stars.forEach((star, index) => {
            if (index < initialRating) {
                star.classList.add('rated');
            }
        });
    }
    stars.forEach(star => {
        star.addEventListener('click', function () {
            const selectedRating = this.dataset.rating;
            ratingValue.value = selectedRating;

            stars.forEach(s => {
                if (s.dataset.rating <= selectedRating) {
                    s.classList.add('rated');
                } else {
                    s.classList.remove('rated');
                }
            });
        });
    });

    const form = rating.closest('form');
    form.addEventListener('submit', function (event) {
        if (!hasConfirmed && initialRating !== parseInt(ratingValue.value)) {
            event.preventDefault(); // Ngăn chặn gửi form mặc định

            const confirmDialog = confirm("Bạn có chắc sẽ đánh giá " + ratingValue.value + " sao. Nếu đánh giá, bạn sẽ không được đánh giá lại nữa.");

            if (confirmDialog) {
                hasConfirmed = true;
                form.submit(); // Gửi form sau khi xác nhận
            }
        }
    });
});