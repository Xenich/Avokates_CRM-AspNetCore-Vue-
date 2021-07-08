function RegisterPaginatorComponent()
{
            // компонент с пагинацией, в него передаётся функция _gotopagemethod - переход на нужную страницу
    Vue.component('paginatorcomponent',
        {
            props: ['_pageCount', '_currentpage', '_gotopagemethod'],
            methods:
            {
                isCurrentPage: function (n)
                {
                    return n == this._currentpage;
                }
            },
            template:
                `<div>
    <template v-if="_pageCount > 1">
        <div class= "row">

            <div class="col">
                <button v-if="_currentpage>1" v-on:click="_gotopagemethod(_currentpage-1)" style="margin-right:20px" >
                    Предыдущая
                </button>
            </div>

            <div class="col">
            <div style="margin:'auto'">
                <template v-if="_pageCount < 6">
                    <span  v-for="n in _pageCount">
                        <button v-if="n==_currentpage" v-bind:style="[{color:'red', 'font-size':'25px', margin:'5px'}]">                        
                            {{ n }}
                        </button>
                        <button v-if="n!=_currentpage" v-bind:style="[{color:'black', margin:'5px'}]" 
                                 v-on:click="_gotopagemethod(n)">
                            {{ n }}
                        </button>
                    </span>
                </template>

                <template v-if="_pageCount > 5">

                        <button v-on:click="_gotopagemethod(1)"
                                v-bind:style="[_currentpage==1?{color:'red', 'font-size':'25px', margin:'5px'}:{color:'black', margin:'5px'}]">
                            1
                        </button>

                        <label>...</label>

                        <button v-if="_currentpage!=1&&_currentpage!=_pageCount" v-bind:style="[{color:'red', 'font-size':'25px', margin:'5px'}]">
                            {{_currentpage}}
                        </button>

                        <label>...</label>

                        <button v-on:click="_gotopagemethod(_pageCount)"
                                v-bind:style="[_currentpage==_pageCount?{color:'red', 'font-size':'25px', margin:'5px'}:{color:'black', margin:'5px'}]">
                            {{_pageCount}}
                        </button>
                </template>
            </div>
            </div>

            <div class="col">
                <button v-if="_currentpage<_pageCount"  v-on:click="_gotopagemethod(_currentpage+1)"  style="margin-left:20px">
                    Следующая
                </button>
            </div>
        </div>
    </template>
</div>`
        });
}