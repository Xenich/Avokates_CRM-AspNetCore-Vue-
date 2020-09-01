$(document).ready(function ()
{
    var vm = new Vue(				
        {
            el: '#cabinetVueModel',			
            data: 					
            {
                Name: '',
                surname: '',
                secondName : '',
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
            model.Name = result.name;
            model.surname = result.surname;
            model.secondName = result.secondName;
            model.publicKey = result.publicKey;
            //localStorage.setItem("privateKey", "privateKey__++==");
            model.privateKey = localStorage.getItem("privateKey");
        }
    )
}

