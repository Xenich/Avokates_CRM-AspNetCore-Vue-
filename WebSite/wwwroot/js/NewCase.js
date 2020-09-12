$(document).ready(function () {

Vue.component('figurant', 			
    {			
        props: ['_figurant'],
        data: function ()			
        {
            return {
                selected : ''
            }
        },
        methods: {
            Remove()
            {
                this._figurant.Remove(this._figurant.id);
            },
            OnChangeSelectedTask()
            {
                this._figurant.selectedRole = this.selected;
            }
        },
        template: `
                    <transition  appear name="bounce" leave-active-class="animated bounceOutRight">
                        <div>
                            <div class="row">
                                <div class="col-3">
                                    <label>Фамилия</label>
					                <input  class="allinputs" type="text" v-model="_figurant.figurantSurname"/>

                                    <label>Имя</label>
					                <input class="allinputs" type="text" v-model="_figurant.figurantName"/>

                                    <label>Отчество</label>
					                <input class="allinputs" type="text" v-model="_figurant.figurantSecondName"/>

                                    <label>Телефон</label>
					                <input class="allinputs" type="text" v-model="_figurant.figurantPhone"/>

                                    <label>Email</label>
					                <input class="allinputs" type="text" v-model="_figurant.figurantEmail"/>

                                    <label>Роль в деле</label>
			                        <select v-model="selected" @change="OnChangeSelectedTask">
				                        <option v-for="option in _figurant.figurantRoleOptions" v-bind:value="option.id" >
				                          {{ option.name }}
				                        </option>
			                         </select>
                                </div>
                                <div class="col-9" style="display: flex; flex-direction:column">
                                    <div>
                                        <label>Детали</label>   
                                        <textarea rows="5" style="width: 100%;" v-model="_figurant.figurantDescription"></textarea>
                                    </div>
                                    <input style="margin-left: auto; margin-top: auto;" type="button" v-on:click="Remove" value="Удалить" />                           
                                </div>
                            </div>
                            <hr>
                        </div>
                    </transition>`                   
    });

    var vm = new Vue(
        {
            el: '#newCase',
            data:
            {
                title: '',
                info: '',
                figurantCounter: 1,
                figurants: [],
                figurantsMap: new Map(),
                roleOptions: []
            },

            methods:
            {
                AddFigurant()
                {
                    this.figurantCounter++;
                    var newFigurant = {
                        id: this.figurantCounter,
                        figurantSurname: '',
                        figurantName: '',
                        figurantSecondName: '',
                        figurantRole: '',
                        figurantDescription: '',  
                        figurantPhone: '',  
                        figurantEmail: '',  
                        figurantRoleOptions: [],
                        selectedRole: ''
                    };
                    newFigurant.figurantRoleOptions = roleOptions;//.slice();
                    newFigurant.Remove = this.RemoveFigurant;
                    this.figurants.push(newFigurant);
                    this.figurantsMap.set(newFigurant.id, newFigurant);

                },
                RemoveFigurant(id)
                {
                    this.figurantsMap.delete(id);
                    this.figurants = Array.from(this.figurantsMap.values());
                    
                },

                CreateNewCase()
                {
                    var array = [];

                    this.figurants.forEach(element =>
                    {
                        var obj =
                        {
                            surname: element.figurantSurname,
                            roleUid: element.selectedRole,
                            name: element.figurantName,
                            secondName: element.figurantSecondName,
                            description: element.figurantDescription,
                            phone: element.figurantPhone,
                            email: element.figurantEmail
                        }
                        array.push(obj);
                    });

                    var body = {
                        title: this.title,
                        info: this.info,
                        figurants: array
                    }

                    DataRequest('CreateNewCase', body, true,
                        function (result) {

                        });
                }
            },       
            created: function ()
            {			
                var lbl = document.getElementById("errorLabel");
                DataRequest('NewCaseGetModel', null, true,
                    function (result)
                    {
                        this.roleOptions = result.figurantRoleOptions;                        
                    },
                    function (errorMsg)
                    {
                        ErrorHandler(lbl, errorMsg)
                    });
            }
        });
})