function RegisterNotesListWithPaginationComponent()
{
        // Компонент списка заметок с пагинацией
    Vue.component('noteslistwithpaginationcomponent',
        {
            props: ['_notestable', '_canmanage', '_caseuid', '_gotopagemethod_'],
            methods:
            {
               
            },
            template: `
<div class="card-body">
    <note v-for="note in _notestable.notes"
            v-bind:note="note"
            v-bind:key="note.id"
            v-bind:canmanage="_canmanage"
            v-bind:candelete="note.canDelete"
            v-bind:caseuid="_caseuid">
    </note>

    <paginatorcomponent
        v-bind:_pageCount="_notestable.pageCount"
        v-bind:_currentpage="_notestable.currentPage"
        v-bind:_gotopagemethod="_gotopagemethod_" >
    </paginatorcomponent>
</div>
            `
        });

}