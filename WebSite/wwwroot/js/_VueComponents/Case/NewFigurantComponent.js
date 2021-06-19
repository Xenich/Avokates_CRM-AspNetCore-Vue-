function RegisterNewFigurantComponent() {
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

                    DataRequest('Data', 'AddNewFigurantToCase', body, true,
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
}