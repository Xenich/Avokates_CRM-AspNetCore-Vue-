$(document).ready(function () {
    var vm = new Vue(				// объект класса vue
        {
            el: '#caseVueModel',				// работаем с элементом с id = 'CaseVueModel'
            data: 					// объект, в котором будут храниться переменные
            {
                caseId: $('#caseId').val(),
                title : ''
            },
            //methods:					// объект, в котором будут хранится функции
            //{
            //    GetCasesList_() {
            //        GetCasesList(this);
            //        //this.casesList = GetCasesList();
            //    }
            //},
            created: function () {			// выполнение кода после создания экземпляра Vue
                GetCaseInfo(this);
            }
        });
})

// подгрузка списка дел
function GetCaseInfo(model) {
    var data = {
        'id': model.caseId
    }
    DataRequest('GetCaseInfo', data, null, true,
        function (result) {
            model.title = result.title;
        });
}
