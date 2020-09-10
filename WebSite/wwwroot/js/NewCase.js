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
        calculated:
        {
            //figurantArrayCounter: this.counter+1
        },
        methods: {
            Remove()
            {
                this._figurant.Remove(this._figurant.id);
            }
        },
        template: `<div>
                    <div class="row">
                        <div class="col-3">
                            <label>Фамилия</label>
					        <input  class="allinputs" type="text" v-model="_figurant.figurantSurname"/>

                            <label>Имя</label>
					        <input class="allinputs" type="text" v-model="_figurant.figurantName"/>

                            <label>Отчество</label>
					        <input class="allinputs" type="text" v-model="_figurant.figurantSecodName"/>

                            <label>Роль в деле</label>
			                <select v-model="selected">
				                <option v-for="option in _figurant.figurantRoleOptions" v-bind:value="option.id">
				                  {{ option.name }}
				                </option>
			                 </select>
                        </div>
                        <div class="col-9">
                            <label>Детали</label>
                            <textarea rows="5" style="width: 100%;">{{_figurant.figurantDescription}}</textarea>
                            <input style="position: absolute; bottom: 10px; right: 10px; " type="button" v-on:click="Remove" value="Удалить" />
                        </div>
                     </div>
                    <hr>
					</div>`
                    
    });

    var vm = new Vue(
        {
            el: '#newCase',
            data:
            {
                title: '',
                description: '',
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
                        figurantSecodName: '',
                        figurantRole: '',
                        figurantDescription: '',  
                        figurantRoleOptions: []
                    };
                    newFigurant.figurantRoleOptions = roleOptions;
                    newFigurant.Remove = this.RemoveFigurant;
                    this.figurants.push(newFigurant);
                    this.figurantsMap.set(this.figurantCounter, newFigurant);

                },
                RemoveFigurant(id)
                {
                    this.figurantsMap.delete(id);
                    this.figurants = Array.from(this.figurantsMap.values());
                    
                },

                CreateNewCase()
                {
                    var body =
                    {

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