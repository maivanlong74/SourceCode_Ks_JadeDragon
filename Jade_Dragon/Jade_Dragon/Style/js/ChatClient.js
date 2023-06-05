$(function () {
    var chat = $.connection.chat;
    loadClient(chat);
    loadDelete(chat);
    loadGroup(chat);

    $.connection.hub.start().done(function () {
        $('#btnSend').click(function () {
            sendMessage(chat);
        });

        $('.btnUser').click(function () {
            var MaNguoiNhan = $(this).data('id');
            var TenNguoiNhan = $(this).text();
            var MaNguoiGui = $('#makh').val();
            ChatUser(chat, MaNguoiNhan, TenNguoiNhan, MaNguoiGui);
        });

        // Đổi phòng
        $('#Create_Room').on("click", "button", (function () {
            var roomId = $(this).data('id');

            var tenphong = $(this).text();
            ChatGroup(chat, roomId, tenphong);
        }));

        // Xử lý sự kiện nhấn phím Enter để gửi tin nhắn
        $('#txtMessage').keypress(function (e) {
            if (e.which === 13) { // Kiểm tra xem phím Enter đã được bấm chưa
                sendMessage(chat);
            }
        });

        // Lấy danh sách tin nhắn trong phòng chat và hiển thị lên
        var roomId = $('#idphong').val();
        if (roomId == null || roomId.trim() == '') {
            var idnhan = $('#idUser').val();
            var idgui = $('#makh').val();
            chat.server.getMessagesAdmin(idnhan, idgui);
        } else {
            chat.server.getMessages(roomId);
        }

        //Lấy danh sách phòng
        chat.server.getTaoMoi();

        // Xóa tin nhắn
        $('#contentMsg').on("click", "button#deleteButton", (function () {
            var matn = $(this).data('id');
            var nguoigui = $(this).data('gui');
            var khachhang = $(this).data('makh');
            chat.server.deleteTinNhan(matn, nguoigui, khachhang);
        }));

    });
});

function sendMessage(chat) {
    var roomId = $('#idphong').val();
    var IdNhan = $('#idUser').val();
    var msg = $('#txtMessage').val();
    var makh = $('#makh').val();
    var fileInput = document.getElementById('fileInput');
    if ((msg == null || msg.trim() == '') && (fileInput.files.length <= 0)) {
        // Nếu msg và file input đều rỗng
        return;
    }
    // Nếu file input không rỗng
    if (fileInput.files.length > 0) {
        for (var i = 0; i < fileInput.files.length; i++) {
            var file = fileInput.files[i];
            var reader = new FileReader();
            reader.onload = function (e) {
                if (msg == null || msg.trim() == '') {
                    if (roomId == null || roomId.trim() == '') {
                        chat.server.message(IdNhan, makh, null, e.target.result, true);
                    } else {
                        chat.server.message(roomId, makh, null, e.target.result, false);
                    }
                } else {
                    if (roomId == null || roomId.trim() == '') {
                        chat.server.message(IdNhan, makh, msg, e.target.result, true);
                    } else {
                        chat.server.message(roomId, makh, msg, e.target.result, false);
                    }
                }
                // Clear file input.
                fileInput.value = '';
            };
            reader.readAsDataURL(file);
        }
    } else {
        if (roomId == null || roomId.trim() == '') {
            chat.server.message(IdNhan, makh, msg, null, true);
        } else {
            chat.server.message(roomId, makh, msg, null, false);
        }
    }
    // Clear message input.
    $('#txtMessage').val('').focus();
}


function ChatGroup(chat, roomId, tenphong) {
    $('#idphong').val(roomId);
    $('#tenphong').text(tenphong);
    $('#contentMsg p').hide();
    $('#contentMsg li').hide(); // hide all messages
    $('#contentMsg li.admin').show(); // show only admin messages

    chat.server.getMessages(roomId);
}

function ChatUser(chat, MaNguoiNhan, TenNguoiNhan, MaNguoiGui) {

    $('#idphong').val("");
    $('#idUser').val(MaNguoiNhan);
    $('#tenphong').text(TenNguoiNhan);
    $('#contentMsg p').hide();
    $('#contentMsg li').hide(); // hide all messages
    $('#contentMsg li.admin').show(); // show only admin messages

    chat.server.getMessagesAdmin(MaNguoiNhan, MaNguoiGui);
}

/*-----------------------------------------------*/

var lastSenderId = null; // khởi tạo biến lưu trữ mã khách hàng ở vòng trước
var lastMessageTime = null; // khởi tạo biến lưu trữ thời gian tin nhắn cuối cùng

function loadClient(chat) {
    chat.client.message = function (name, msg, makh, ngaygui, matinnhan, imageUrl) {
        var MaKhachHang = $('#makh').val();
        var li = "";
        var messageTime = new Date(ngaygui).getTime();
        var linkanh = "/Style/img/icon/icon-X.jpg";

        // Kiểm tra nếu đã đủ 1 giờ kể từ lần gửi tin nhắn cuối cùng
        if (lastMessageTime && (messageTime - lastMessageTime) >= (60 * 60 * 1000)) {
            // Nếu đã đủ 1 giờ, in ra thời gian của tin nhắn mới
            var messageDate = new Date(messageTime);
            var messageDateString = messageDate.toLocaleString();
            var messageTimeHTML = '<p class= "timenow" >' + messageDateString + '</p>';
            $('#contentMsg').append(messageTimeHTML);

            if (makh == $('#makh').val()) { // người gửi tin nhắn là người dùng đang truy cập
                li = "<li data-sender='" + makh + "' class = 'me' >" +
                    "<div class='Delete'><button type='button' id='deleteButton' data-id='" + matinnhan + "' data-gui='" + makh + "' data-makh='" + MaKhachHang + "'>" +
                    "<img src='" + linkanh + "' title='Xóa tin nhắn'/>" +
                    "</button></div > " +
                    (msg ? "<span>" + msg + "</span>" : "") +
                    (imageUrl ? '<br/><img src="' + imageUrl + '" />' : '') + "</li>";
            } else {
                li = "<li data-sender='" + makh + "' class = 'you' ><p>" + name + "</p>" +
                    (msg ? "<span>" + msg + "</span>" : "") +
                    (imageUrl ? '<br/><img src="' + imageUrl + '" />' : '') + "</li>";
            }
            $('#contentMsg').append(li);

        } else {
            if (makh == $('#makh').val()) { // người gửi tin nhắn là người dùng đang truy cập
                li = "<li data-sender='" + makh + "' class = 'me' >" +
                    "<div class='Delete'><button type='button' id='deleteButton' data-id='" + matinnhan + "' data-gui='" + makh + "' data-makh='" + MaKhachHang + "'>" +
                    "<img src='" + linkanh + "' title='Xóa tin nhắn'/>" +
                    "</button></div > " +
                    (msg ? "<span>" + msg + "</span>" : "") +
                    (imageUrl ? '<br/><img src="' + imageUrl + '" />' : '') + "</li>";
            } else {
                li = "<li data-sender='" + makh + "' class = 'you' ><p>"
                    + (makh !== lastSenderId ? name + ":" : "") + "</p>" +
                    (msg ? "<span>" + msg + "</span>" : "") +
                    (imageUrl ? '<br/><img src="' + imageUrl + '" />' : '') + "</li>";
            }
            $('#contentMsg').append(li);
        }

        lastSenderId = makh; // lưu mã khách hàng hiện tại cho lần so sánh ở vòng sau
        lastMessageTime = messageTime; // lưu lại thời gian gửi tin nhắn cuối cùng

        var hasImage = imageUrl !== null && imageUrl !== '';
        if (hasImage) {
            setTimeout(function () {
                var messagesContainer = $('#contentMsg');
                messagesContainer.scrollTop(messagesContainer[0].scrollHeight);
            }, 500); // chờ 500ms để hình ảnh được tải xong
        } else {
            var messagesContainer = $('#contentMsg');
            messagesContainer.scrollTop(messagesContainer[0].scrollHeight);
        }
    }
}

function loadDelete(chat) {
    chat.client.tinNhanDaXoa = function (id, kh, loai) {
        // Lấy danh sách tin nhắn trong phòng chat và hiển thị lên
        if (loai == true) {
            $('#contentMsg').empty();
            chat.server.getMessagesAdmin(id, kh);
        } else {
            chat.server.getMessages(id);
            $('#contentMsg').empty();
        }
    };
}

function loadGroup(chat) {
    chat.client.taoMoi = function (maphong, tenphong, tenks) {
        if (tenks != null) {
            var ten = "Khách sạn " + tenks;
            var ht = "<li><button type='button' title='" + ten + "' class='btnChatPhong' data-id='" + maphong + "'>" +
                tenphong + "</button></li>"
        } else {
            var ht = "<li><button type='button' class='btnChatPhong' data-id='" + maphong + "'>" +
                tenphong + "</button></li>"
        }
        $('#Create_Room').append(ht);
    }
}