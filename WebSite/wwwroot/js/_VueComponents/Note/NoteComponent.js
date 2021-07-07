function RegisterNoteComponent()
{
        // компонент записи по делу
    Vue.component('note',
        {
            props: ['note', 'canmanage', 'candelete', 'caseuid'],
            methods:
            {
                Remove() {
                    var body =
                    {
                        caseUid: this.caseuid,
                        noteUid: this.note.uid
                    }
                    DataRequest('Data', 'RemoveNoteFromCase', body, true,
                        function (result) {
                            location.reload();
                        });
                }
            },
            template: `
                <div>
                    <div class="row">
                        <h6  class="col-6">Запись № {{note.id}}</h6>
                        <label class="col-6">Дата: {{note.date}}</label>  
                    </div>
                    <div class="row">
                        <div class="col-6"></div>
                        <label class="col-6">Добавил: {{note.employeeInfo}}</label>  
                    </div>
                    <h5 class="row">{{note.title}}</h5>
                    <div class="row">
                        <div class="col-1"></div>
                        <label  class="col-11"  style="background-color:#f1f1f1"> {{note.text}}</label>
                        <!-- <textarea class="col-11" rows="20" style="width: 100%;" v-model="note.text"></textarea> -->
                    </div>
                    <input  v-if='candelete' type="button" v-on:click="Remove" value="Удалить запись" />
                    <hr>
                </div>
            `
        });
}
