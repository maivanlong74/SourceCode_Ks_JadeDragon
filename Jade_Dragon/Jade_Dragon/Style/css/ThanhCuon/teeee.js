const stars = document.querySelectorAll('.star');
const ma = document.querySelector('.maks');
const maks = ma.value;
var className = "rating-value_" + maks;
const ratingValue = document.querySelector('.' + className);
// Set initial rating value
ratingValue.value = parseInt(ratingValue.getAttribute('value'));
if (ratingValue.value != "NaN") {
    for (let i = 0; i < ratingValue.value; i++) {
        stars[i].classList.add('rated');
    }
    stars.forEach(star => {
        star.addEventListener('click', function () {
            alert('Bạn đã đánh giá ' + ratingValue.value + ' sao rồi. \n'
                + 'Bạn không thể thay đổi đánh giá nữa');
        });
    });
} else {
    stars.forEach(star => {
        star.addEventListener('click', function () {
            ratingValue.value = this.dataset.rating;

            if (confirm("Bạn có chắc sẽ đánh giá " + ratingValue.value + " sao không? ")) {
                stars.forEach(s => s.classList.remove('rated'));
                for (let i = 0; i < this.dataset.rating; i++) {
                    stars[i].classList.add('rated');
                }
            } else {
                return;
            }
        });
    });
}

observer.observe(ratingValue, { attributes: true });