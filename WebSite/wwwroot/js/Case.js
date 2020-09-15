$(document).ready(function () {

        // Компонент сотрудника с доступом к данному делу
    Vue.component('employeewithaccess',
        {
            props: ['employee', 'caseuid'],
            methods: {
                RemoveAccess() {
                    console.log("Закрыт доступ:" + this.employee.employeeUid);
                }
            },
            template: `<div class="row">
                        <label class="col-8">{{employee.name}}</label>
                        <input  v-if='employee.canManageThisEmployee' class="col" type="button" v-on:click="RemoveAccess" value="Закрыть доступ" />
                       </div>`
        });
    
    // Компонент сотрудника без доступом к данному делу
    Vue.component('employeewithoutaccess',
        {
            props: ['employee', 'caseuid', 'useruid'],
            methods: {
                GrantAccess() {
                    var body =
                    {
                        'userUid': this.employee.employeeUid,
                        'caseUid': this.caseuid,
                        'privateKey': localStorage.getItem("privateKey" + this.useruid)
                    };

                    DataRequest('GrantAccessToCase', body, true,
                        function (result) {

                        });                   
                }
            },
            template: `<div class="row">
                        <label class="col-8">{{employee.name}}</label>
                        <input v-if='employee.canManageThisEmployee' class="col" type="button" v-on:click="GrantAccess" value="Открыть доступ" />
                       </div>`
        });

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
                notes: []
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


function GetCaseInfo(model) {
    var data = {
        'id': model.caseId,
        'privateKey': localStorage.getItem("privateKey" + model.userUid)
    }
    var lbl = document.getElementById("errorLabel");
    DataRequest('GetCaseInfo', data, true,
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
            //model.hasAccess = result.hasAccess;
        },
        function (errorMsg) {
            ErrorHandler(lbl, errorMsg)
        });
}
