function RegisterNewNoteComponent()
{
        // компонент новой записи по делу
    Vue.component('newnote',
        {
            props: ['parentmodel'],
            data: function () {
                return {
                    notetext: '',
                    notetitle: '',
                    files: []
                }
            },
            methods: {
                Add() {
                    var model = this.parentmodel;
                    var data = new FormData();
                    for (var i = 0; i != this.files.length; i++) {
                        data.append("files", this.files[i]);
                    };
                    data.append("files", this.files);
                    data.append("caseUid", this.parentmodel.caseUid);
                    data.append('title', this.notetitle);
                    data.append('text', this.notetext);
                    data.append('privateKey', localStorage.getItem("privateKey" + this.parentmodel.userUid));
                    $.ajax({
                        url: '/Data/AddNewNoteToCase/',
                        type: 'POST',
                        data: data,
                        cache: false,
                        dataType: 'json',
                        processData: false, // Не обрабатываем файлы (Don't process the files)
                        contentType: false, // Так jQuery скажет серверу что это строковой запрос
                        success: function (respond, textStatus, jqXHR) {
                            GetCaseNotes(model);
                        }
                    });
                },
                Cancel() {
                    this.parentmodel.newNoteCreating = false;
                    this.parentmodel.newNoteNotCreating = true;
                },
                Changed() {
                    var input = $('#inputFiles')[0];
                    this.files = input.files;
                    console.log("ИЗМЕНЕНО");
                }
            },
            template: `
                    <transition  appear name="bounce" leave-active-class="animated bounceOutRight">
                        <div>
                            <h2>Новая запись по делу</h2>
                            <div class="row">
                                <label>Заголовок</label>   
                                <textarea rows="2" style="width: 100%;" v-model="notetitle"></textarea>
                            </div>
                            <input type="file" id="inputFiles" name="uploadedFile" multiple accept="*" v-on:change="Changed()">
                            <div class="row">
                                <div class="col-12" style="display: flex; flex-direction:column">
                                    <div>
                                        <label>Текст</label>   
                                        <textarea rows="10" style="width: 100%;" v-model="notetext"></textarea>
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
}