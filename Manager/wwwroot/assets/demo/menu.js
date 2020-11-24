

function GetMenu(id) {
    $.ajax({
        type: "GET",
        url: "/Home/SelectMenu",
        data: { id: id },
        success: function (data) {
            if (data.id != null) {
                window.location = '/' + data.id;
            }
        }
    });
};
$('#nav_menu li').on('click', function () {
    GetMenu(this.id + '');
});
$('#_menu li a').on('click', function () {
    GetMenu(this.id + '');
});