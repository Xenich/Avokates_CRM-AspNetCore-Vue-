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
                CreateUserByInvite() {
                    var body =
                    {
                        login: this.login,
                        password: this.password,
                        invitingToken: $('#inviteToken').html()
                    }
                    DataRequest('CreateUserByInvite', body, true,
                        function (result)
                        {
                            localStorage.setItem("privateKey" + result.userUid, result.privateKey);
                            location.href = '/Home/Cabinet';
                        });
                }
            }
        });
})