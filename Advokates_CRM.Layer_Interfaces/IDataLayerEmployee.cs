using Advokates_CRM_DTO.Inputs;
using Advokates_CRM_DTO.Outputs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Advokates_CRM.Layer_Interfaces
{
    public interface IDataLayerEmployee
    {
        // создание приглашения новому пользователю путём отсылки ему email с токеном приглашения
        ResultBase CreateInvite(string token, string email);
        // Форма регистрации нового пользователя по пригласительному токену, полученному по email
        InviteResult Invite(string token);
        //Регистрация нового пользователя по пригласительному токену, отправленному на имейл
        Registration_Out CreateUserByInvite(Registration_In value);
        string GetUserUidByJWT(string token);
    }
}
