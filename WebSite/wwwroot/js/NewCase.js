$(document).ready(function () {
    var vm = new Vue(
        {
            el: '#newCase',
            data:
            {
                title: '',
                description:''
            },

            methods:
            {
                CreateNewCase() {
                    var body =
                    {

                    }
                    DataRequest('CreateNewCase', body, true,
                        function (result) {

                        });
                }
            }
        });
})