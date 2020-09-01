$(document).ready(function ()
{
    var vm = new Vue(				
        {
            el: '#cabinetVueModel',			
            data: 					
            {
                name: '',
                surname: '',
                secondName: '',
                birthday:'',
                email: '',
                phone: '',
                role:'',
                publicKey: '',
                privateKey:''
            },

            methods:					
            {
                SaveChanges() {
                    
                }
            },

            created: function ()
            {			
                GetCabinetInfo(this);    
                var r = 1;
            }
        });
})

function GetCabinetInfo(model)
{
    DataRequest('GetCabinetInfo', null, true,
        function (result) {
            model.name = result.name;
            model.surname = result.surname;
            model.secondName = result.secondName;
            model.birthday = result.birthday;
            model.email = result.email;
            model.phone = result.phone;
            model.role = result.role;
            model.publicKey = result.publicKey;
            //localStorage.setItem("privateKey", "privateKey__++==");
            model.privateKey = localStorage.getItem("privateKey");
        }
    )
}

