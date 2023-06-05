const ratings = document.querySelectorAll('.rating');

ratings.forEach(rating => {
    const stars = rating.querySelectorAll('.star');
    const ratingValue = rating.querySelector('.rating-value');
    const initialRating = parseInt(ratingValue.value);

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
});