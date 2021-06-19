function RegisterEmployeeWithoutAccessComponent()
{
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

                    DataRequest('Data', 'GrantAccessToCase', body, true,
                        function (result) {
                            location.reload();
                        });
                }
            },
            template: `<div>
                            <hr>
                            <div class="row">
                                <label class="col-8">{{employee.name}}</label>
                                <input v-if='employee.canManageThisEmployee' class="col" type="button" v-on:click="GrantAccess" value="Открыть доступ" />
                            </div>
                       </div>`
        });
}