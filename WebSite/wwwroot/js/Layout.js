$(document).ready(function () {
    var vueModel = new Vue
        ({
            el: '#bodyVueModel',		// работаем с элементом с id = 'header'
            data: 					    // объект, в котором будут храниться переменные
            {
                CompanyName: '',
                Name: '',
                CasesCount: '',
                NotesCount:''
            },

            created: function ()			// выполнение кода после создания экземпляра Vue
            {
                GetLayoutModel(this);
            }
        });
});

function GetLayoutModel(model)
{
    DataRequest('GetMainPage', null, true, 
        function (result) {
            model.CompanyName = result.companyName;
            model.Name = result.name;
            model.CasesCount = result.casesCount;
            model.NotesCount = result.notesCount;
            if (result.isAdmin)
            {
                var emp = document.getElementById('employeesLink');
                var employeesLink = document.createElement('a');
                employeesLink.setAttribute("href", "/Home/Employees");
                employeesLink.innerHTML = "Сотрудники";
                emp.appendChild(employeesLink);
            }
        }
        //function (errorMsg) {
        //ErrorHandler(lbl, errorMsg)
    );
    //, null);
}


