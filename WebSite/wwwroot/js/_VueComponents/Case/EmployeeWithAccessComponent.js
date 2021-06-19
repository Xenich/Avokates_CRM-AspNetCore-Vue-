function RegisterEmployeeWithAccessComponent()
{
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

                    DataRequest('Data', 'RemoveAccessToCase', body, true,
                        function (result) {
                            location.reload();
                        });
                }
            },
            template: `<div>
                        <hr>
                        <div class="row">
                            <label class="col-8">{{employee.name}}</label>
                            <input v-if='employee.canManageThisEmployee' class="col" type="button" v-on:click="RemoveAccess" value="Закрыть доступ" />
                        </div>
                    </div>`
        });
}