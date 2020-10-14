$(document).ready(function () {

        // Компонент сотрудника с доступом к данному делу
    Vue.component('employeewithaccess',
        {
            props: ['employee', 'caseuid'],
            methods: {
                RemoveAccess() {
                    var body =
                    {
                        'userUid': this.employee.employeeUid,
                        'caseUid': this.caseuid,
                    };

                    DataRequest('RemoveAccessToCase', body, true,
                        function (result) {
                            location.reload();
                        });  
                }
            },
            template: `<div>
                            <hr>
                            <div class="row">
                                <label class="col-8">{{employee.name}}</label>
                                <input  v-if='employee.canManageThisEmployee' class="col" type="button" v-on:click="RemoveAccess" value="Закрыть доступ" />
                            </div>
                       </div>`
        });
//------------------------------------------------------------------------------------------------------------------------------  
    // Компонент сотрудника без доступом к данному делу
    Vue.component('employeewithoutaccess',
        {
            props: ['employee', 'caseuid', 'useruid'],
            methods: {
                GrantAccess()
                {
                    var body =
                    {
                        'userUid': this.employee.employeeUid,
                        'caseUid': this.caseuid,
                        'privateKey': localStorage.getItem("privateKey" + this.useruid)
                    };

                    DataRequest('GrantAccessToCase', body, true,
                        function (result) {
                            location.reload();
                        });                   
                }
            },
            template:  `<div>
                            <hr>
                            <div class="row">
                                <label class="col-8">{{employee.name}}</label>
                                <input v-if='employee.canManageThisEmployee' class="col" type="button" v-on:click="GrantAccess" value="Открыть доступ" />
                            </div>
                       </div>`
        });
//------------------------------------------------------------------------------------------------------------------------------
    // компонент - таблица фигурантов по делу
    Vue.component('figurantstable',
        {
            props: ['figurants', 'canmanage','caseuid'],
            methods:
            {
                Remove(figurantUid)
                {
                    var body =
                    {                       
                        figurantUid: figurantUid,
                        caseUid: this.caseuid
                    }
                    DataRequest('RemoveFigurantFromCase', body, true,
                        function (result)
                        {
                            location.reload();
                        });
                }
            },
            template: `
            <table class="table table-striped table-bordered table-scrolling" width="100%" cellspacing="0">
                <thead style="">
                    <tr>
                        <th>
                            Ф.И.О.
                        </th>
                        <th>
                            Телефон
                        </th>
                        <th>
                            Роль в деле
                        </th>
                        <th>
                        </th>
                    </tr>
                </thead>
                <tbody>
                    <tr v-for="figurant in figurants">
                        <td>{{figurant.fullName}}</td>
                        <td>{{figurant.phone}}</td>
                        <td>{{figurant.role}}</td>
                        <td><div>
                            <input v-if="canmanage" type="button" v-on:click="Remove(figurant.uid)" value="Удалить" /></div>
                        </td>
                    </tr>
                </tbody>
            </table>`
        });
//------------------------------------------------------------------------------------------------------------------------------
    // компонент нового фигуранта
    Vue.component('newfigurant',
        {
            props: ['parentmodel'],
            data: function () {
                return {
                    figurantSurname: '',
                    figurantName: '',
                    figurantSecondName: '',
                    figurantRole: '',
                    figurantDescription: '',
                    figurantPhone: '',
                    figurantEmail: '',
                    figurantRoleOptions: [],
                    selectedRole: ''
                }
            },
            methods: {
                Add() {
                    var body =
                    {
                        'figurant':
                        {
                            'figurants':
                            [
                                {
                                    name: this.figurantName,
                                    surname: this.figurantSurname,
                                    secondName: this.figurantSecondName,
                                    description: this.figurantDescription,
                                    phone: this.figurantPhone,
                                    email: this.figurantEmail,
                                    roleUid: this.selectedRole,
                                }
                            ]
                        },
                        privateKey: localStorage.getItem("privateKey" + this.parentmodel.userUid),
                        caseUid: this.parentmodel.caseUid,

                    }

                    DataRequest('AddNewFigurantToCase', body, true,
                        function (result) {
                            location.reload();
                        });
                },
                Cancel() {
                    this.parentmodel.newFigurantCreating = false;
                    this.parentmodel.newFigurantNotCreating = true;
                }
            },
            template: `
                    <transition  appear name="bounce" leave-active-class="animated bounceOutRight">
                        <div>
                            <div class="row">
                                <div class="col-4">
                                    <p>Фамилия</p>
					                <input  class="allinputs" type="text" v-model="figurantSurname"/>

                                    <p>Имя</p>
					                <input class="allinputs" type="text" v-model="figurantName"/>

                                    <p>Отчество</p>
					                <input class="allinputs" type="text" v-model="figurantSecondName"/>

                                    <p>Телефон</p>
					                <input class="allinputs" type="text" v-model="figurantPhone"/>

                                    <p>Email</p>
					                <input class="allinputs" type="text" v-model="figurantEmail"/>

                                    <p>Роль в деле</p>
			                        <select v-model="selectedRole">
				                        <option v-for="option in parentmodel.figurantRoleOptions" v-bind:value="option.id">
				                          {{ option.name }}
				                        </option>
			                         </select>
                                </div>
                                <div class="col-8" style="display: flex; flex-direction:column">
                                    <div>
                                        <label>Детали</label>   
                                        <textarea rows="10" style="width: 100%;" v-model="figurantDescription"></textarea>
                                    </div>
                                    <div  style="margin-left: auto; margin-top: auto;">
                                        <input type="button" v-on:click="Cancel" value="Отмена" />  
                                        <input type="button" v-on:click="Add" value="Сохранить" />  
                                    </div>
                                </div>
                            </div>
                            
                        </div>
                    </transition>`
        });
//------------------------------------------------------------------------------------------------------------------------------
   
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
            methods:					// объект, в котором будут хранится функции
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
            model.newFigurantCreating = false;
            model.newFigurantNotCreating = true;
            model.figurantRoleOptions = result.figurantRoleOptions;
            //model.hasAccess = result.hasAccess;
        },
        function (errorMsg) {
            ErrorHandler(lbl, errorMsg)
        });
}

