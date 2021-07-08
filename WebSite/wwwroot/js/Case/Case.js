$(document).ready(function () {
        // Регистрация компонентов
    RegisterEmployeeWithAccessComponent();          // Компонент сотрудника с доступом к данному делу
    RegisterEmployeeWithoutAccessComponent();       // Компонент сотрудника без доступом к данному делу
    RegisterFigurantsTableComponent();              // компонент - таблица фигурантов по делу
    RegisterNewFigurantComponent();                 // компонент нового фигуранта
    RegisterNotesListWithPaginationComponent();     // компонент списка заметок с пагинацией
    RegisterPaginatorComponent();                   // компонент с пагинацией
    RegisterNoteComponent();                        // компонент записи по делу

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
                notesTable:
                    {
                        notes: [],
                        pageCount: 1,  
                        elementsCount: 5,
                        currentPage: 1
                    },                

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
                },

                GotoPage: function (currentPage)
                {
                    this.notesTable.currentPage = currentPage;
                    GetCaseNotes(this);
                }
            },
            created: function () {			// выполнение кода после создания экземпляра Vue
                GetCaseInfo(this);
                GetCaseNotes(this);
            }
        });
})


function GetCaseInfo(model)
{
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
            //model.notes = result.notes;
            model.newFigurantCreating = false;
            model.newFigurantNotCreating = true;
            model.figurantRoleOptions = result.figurantRoleOptions;
            //model.hasAccess = result.hasAccess;
        },
        function (errorMsg) {
            ErrorHandler(lbl, errorMsg)
        });
}


function GetCaseNotes(model)
{
    var data = {
        'caseIdPerCompany': model.caseId,
        'privateKey': localStorage.getItem("privateKey" + model.userUid),
        'elementsCount': model.notesTable.elementsCount,
        'currentPage': model.notesTable.currentPage
    }
    var lbl = document.getElementById("errorLabel");
    DataRequest('Data', 'GetCaseNotes', data, true,
        function (result) {
            model.notesTable.notes = result.notes;
            model.newNoteCreating = false;
            model.newNoteNotCreating = true;
            model.notesTable.pageCount = result.pageCount;
        },
        function (errorMsg) {
            ErrorHandler(lbl, errorMsg)
        });
}