$(document).ready(function () {
        // Регистрация компонентов
    RegisterEmployeeWithAccessComponent();          // Компонент сотрудника с доступом к данному делу
    RegisterEmployeeWithoutAccessComponent();       // Компонент сотрудника без доступом к данному делу
    RegisterFigurantsTableComponent();              // компонент - таблица фигурантов по делу
    RegisterNewFigurantComponent();                 // компонент нового фигуранта
   
//------------------------------------------------------------------------------------------------------------------------------

    var vm = new Vue(				// объект класса vue
        {
            el: '#caseVueModel',				// работаем с элементом с id = 'CaseVueModel'
            data: 					// объект, в котором будут храниться переменные
            {
                caseId: $('#caseId').val(),
                userUid: $('#userUid').val(),
                caseUid:'',
                title: '',
                info: '',
                dateCreate: '',
                updateDate: '',
                isClosed: '',
                //hasAccess:false,
                canManage:false,
                employeesWithAccess: [],
                employeesWithoutAccess: [],
                figurants: [],
                newFigurantCreating: false,
                newFigurantNotCreating: true,
                notes: [],
                newNoteCreating: false,
                newNoteNotCreating: true,
                figurantRoleOptions : []
            },
            methods:					// объект, в котором будут храниться функции
            {
                AddFigurant()
                {
                    this.newFigurantCreating = true;
                    this.newFigurantNotCreating = false;
                },
                AddNote()
                {
                    this.newNoteCreating = true;
                    this.newNoteNotCreating = false;
                }
            },
            created: function () {			// выполнение кода после создания экземпляра Vue
                GetCaseInfo(this);
            }
        });
})


function GetCaseInfo(model) {
    var data = {
        'id': model.caseId,
        'privateKey': localStorage.getItem("privateKey" + model.userUid)
    }
    var lbl = document.getElementById("errorLabel");
    DataRequest('Data', 'GetCaseInfo', data, true,
        function (result) {
            model.caseUid = result.caseUid,
            model.title = result.title;
            model.info = result.info;
            model.dateCreate = result.dateCreate;
            model.updateDate = result.updateDate;
            model.isClosed = result.isClosed;
            model.canManage = result.canManage;
            model.employeesWithAccess = result.employeesWithAccess;
            model.employeesWithoutAccess = result.employeesWithoutAccess;
            model.figurants = result.figurants;
            model.notes = result.notes;
            model.newFigurantCreating = false;
            model.newFigurantNotCreating = true;
            model.figurantRoleOptions = result.figurantRoleOptions;
            //model.hasAccess = result.hasAccess;
        },
        function (errorMsg) {
            ErrorHandler(lbl, errorMsg)
        });
}

