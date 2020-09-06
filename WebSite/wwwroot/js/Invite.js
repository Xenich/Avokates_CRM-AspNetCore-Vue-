$(document).ready(function () {
    var vm = new Vue(
        {
            el: '#createNewUserVueModel',
            data:
            {
                login: '',
                password: ''
            },

            methods:
            {
                CreateUser() {
                    var body =
                    {
                        login: this.login,
                        password: this.password,
                        inviteToken: $('#inviteToken').html()
                    }
                    DataRequest('CreateUserByInvite', body, true,
                        function (result) {
                           // location.reload();
                        });
                }
            }
        });
})