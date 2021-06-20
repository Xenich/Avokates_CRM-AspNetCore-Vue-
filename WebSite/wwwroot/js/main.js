// method - метод в контроллере Data, на который идёт запрос
// body - тело запроса
// errorLabel - label, в которую выводится ошибка
// isAsync - bool переменная, показывающая, будет ли запрос асинхронным
// func - функция которая должна вызваться после получения ответа от сервера
// errorHandlerFunction - 
function DataRequest(controller, method, body, isAsync, func)
{
    //errorLabel.style.display = 'block';
    var res;
    $.ajax(
        {
            type: 'POST',
            url: '/' + controller+'/' + method + '/',
            data: body,
            async: isAsync
        })
        .done(function (result)
        {           
            var errorMsg = result.errorMessages;
            var status = result.status;
            if (status == 'ok')
            {
                func(result);
                if (errorMsg.length != 0)
                    ErrorHandler(errorMsg);                
            }
            else
            {
                ErrorHandler(errorMsg);
            }
        })
        .fail(function (e)
        {
            ErrorHandler(null);
        });
}

function ErrorHandler(errorMsg)
{
    var errormes = '';
    if (errorMsg == null)
    {
        errormes = "Произошла ошибка! ";
    }
    else
    {
        for (var i = 0; i < errorMsg.length; i++)
        {
            if (errorMsg[i].message != '' && errorMsg[i].message != null)
                errormes += errorMsg[i].message + "  ";
            if (errorMsg[i].errorMessage != '' && errorMsg[i].errorMessage != null)
                errormes += errorMsg[i].errorMessage + "; ";
        }
    }
    errorLabel.innerHTML = errormes;
    errorLabel.style.display = "block";
}

