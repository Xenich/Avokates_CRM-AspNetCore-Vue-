function RegisterFigurantsTableComponent()
{
        // компонент - таблица фигурантов по делу
    Vue.component('figurantstable',
        {
            props: ['figurants', 'canmanage', 'caseuid'],
            methods:
            {
                Remove(figurantUid) {
                    var body =
                    {
                        figurantUid: figurantUid,
                        caseUid: this.caseuid
                    }
                    DataRequest('Data', 'RemoveFigurantFromCase', body, true,
                        function (result) {
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
}
