// method - метод в контроллере Data, на который идёт запрос
// body - тело запроса
// errorLabel - label, в которую выводится ошибка
// isAsync - bool переменная, показывающая, будет ли запрос асинхронным
// func - функция которая должна вызваться после получения ответа от сервера
// errorHandlerFunction - 
function DataRequest(method, body, isAsync, func)
{
    //errorLabel.style.display = 'block';
    var res;
    $.ajax(
        {
            type: 'POST',
            url: '/Data/' + method + '/',
            data: body,
            async: isAsync
        })
        .done(function (result)
        {
            
            var errorMsg = result.errorMessages;
            if (errorMsg == null) {
                func(result);
            }
            else
            {
                ErrorHandler( errorMsg);
            //    if (errorLabel != null)
            //    {
            //        var errormes = "Произошла ошибка! ";
            //        for (var i = 0; i < errorMsg.length; i++) {
            //            errormes += errorMsg[i].message + "  " + errorMsg[i].errorMessage + "; ";
            //        }


            //        errorLabel.style.display = 'block';
            //        errorLabel.innerHTML = errormes;
            //    }
            }
        })
        .fail(function (e) {
            ErrorHandler(null);
            //if (errorLabel != null) {
            //    var errormes = "Произошла ошибка! ";
            //    errorLabel.innerHTML = errormes;
            //   errorLabel.style.display = "block";
            //} 
        });
}

function ErrorHandler(errorMsg)
{
    
    if (errorMsg == null)
    {
        var errormes = "Произошла ошибка! ";
    }
    else
    {
        for (var i = 0; i < errorMsg.length; i++)
        {
            errormes += errorMsg[i].message + "  " + errorMsg[i].errorMessage + "; ";
        }
    }
    errorLabel.innerHTML = errormes;
    errorLabel.style.display = "block";
}

