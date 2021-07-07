function RegisterNotesListWithPaginationComponent()
{
        // Компонент списка заметок с пагинацией
    Vue.component('noteslistwithpaginationcomponent',
        {
            props: ['_notes', '_canManage', '_caseUid'],
            methods:
            {
               
            },
            template: `
<div class="card-body">
    <note v-for="note in _notes"
            v-bind:note="note"
            v-bind:key="note.id"
            v-bind:canmanage="_canManage"
            v-bind:candelete="note.canDelete"
            v-bind:caseuid="_caseUid">
    </note>
</div>
            `
        });

}