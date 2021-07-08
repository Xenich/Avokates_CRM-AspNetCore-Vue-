$(document).ready(function ()
{
    RegisterNoteComponent();                // компонент записи по делу
    RegisterNewNoteComponent();             // компонент новой записи по делу

})

/*
function GetCaseNotes(model) {
    var data = {
        'caseUid': model.caseUid,
        'privateKey': localStorage.getItem("privateKey" + model.userUid)
    }
    var lbl = document.getElementById("errorLabel");
    DataRequest('Data', 'GetCaseNotes', data, true,
        function (result) {
            model.notes = result.notes;
            model.newNoteCreating = false;
            model.newNoteNotCreating = true;
        },
        function (errorMsg) {
            ErrorHandler(lbl, errorMsg)
        });
}
*/