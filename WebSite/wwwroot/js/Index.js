$(document).ready(function ()
{
    var vm = new Vue (				// объект класса vue
    {
            el: '#CasesTableBody',				// работаем с элементом с id = 'CasesTableBody'
            data: 					// объект, в котором будут храниться переменные
            {
                casesList: []
            },
            //methods:					// объект, в котором будут хранится функции
            //{
            //    GetCasesList_() {
            //        GetCasesList(this);
            //        //this.casesList = GetCasesList();
            //    }
            //},
            created: function () {			// выполнение кода после создания экземпляра Vue
                GetCasesList(this);
            }	
    });
})

// подгрузка списка дел
function GetCasesList(model)
{
    var lbl = document.getElementById("errorLabel");
    var result;
    DataRequest('Data', 'GetCasesList', null, true,
        function (result) {
            model.casesList = result.casesList;
        },
        function (errorMsg) {
            ErrorHandler(lbl, errorMsg)
        });
}
