$(document).ready(function () {
    var vm = new Vue(
        {
            el: '#emloyeesVueModel',
            data:
            {
                inviteEmail: ''
            },

            methods:
            {
                SendInviteEmail() {
                    var body =
                    {
                        email: this.inviteEmail
                        
                    }
                    DataRequest('CreateInvite', body, true,
                        function (result) {
                            location.reload();
                        });
                }
            }
        });
})