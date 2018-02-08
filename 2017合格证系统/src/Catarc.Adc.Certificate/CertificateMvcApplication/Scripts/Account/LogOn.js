function login() {
    var username = $("#username").val();
    var password = $("#password").val();
    $.ajax({
        method: 'GET',
        url: '/Account/LogOn?UserName=' + username + '&Password=' + password,
    });
}