const dropArea = document.querySelector('.khuvuc');
/*const Text = dropArea.querySelector('header');*/
const Button = dropArea.querySelector('.button-file');
const Input = dropArea.querySelector('.input-file');

Button.addEventListener('click', () => {
    Input.click();
})

Input.addEventListener('change', function () {
    const file = this.files[0];
    showFile(file);
})

dropArea.addEventListener('dragover', (event) => {
    event.preventDefault();
    Text.textContent = "Thả để tải ảnh"
})

dropArea.addEventListener('dragleave', (event) => {
    event.preventDefault();
    Text.textContent = "Kéo và thả để tải ảnh"
})

dropArea.addEventListener('drop', (event) => {
    event.preventDefault();
    const file = event.dataTransfer.files[0];
    showFile(file);
})

function showFile(file) {
    const fileType = file.type;
    const valiExtensions = ['image/jpeg', 'image/jpg', 'image/png'];
    if (valiExtensions.includes(fileType)) {
        const fileReader = new FileReader();
        fileReader.onload = () => {
            console.log('ổnnnnnn')
            const fileUrl = fileReader.result;
            const imgTag = `<img src="${fileUrl}">`;
            dropArea.innerHTML = imgTag
        }
        fileReader.readAsDataURL(file);
    } else {
        alert("Đây không phải là file ảnh");
        Text.textContent = "Kéo và thả để tải ảnh"
    }
}