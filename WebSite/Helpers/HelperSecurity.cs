using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

using System.Text;
using System.Security.Claims;
using System.Linq;
using Avokates_CRM.Models.Outputs;
using WebSite.Models.Outputs;

namespace WebSite.Helpers
{
    public static class HelperSecurity
    {
        static IConfiguration Configuration;
        public static void Init(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static string GenerateToken(Authorization_Out_FromDB auth)
        {
            List<Claim> claimList = new List<Claim>();

            claimList.Add(new Claim("employeeId", auth.EmployeeId.ToString()));
            claimList.Add(new Claim("employeeUid", auth.EmployeeUid.ToString()));
            claimList.Add(new Claim("login", auth.Login));
            claimList.Add(new Claim("companyId", auth.CompanyId.ToString()));
            claimList.Add(new Claim("userName", string.IsNullOrEmpty(auth.Name)?"": auth.Name));
            claimList.Add(new Claim("roleName", string.IsNullOrEmpty(auth.RoleName) ? "" : auth.RoleName));

            SecurityKey KEY = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("secretKey").Get<string>()));
            SigningCredentials signingCredentials = new SigningCredentials(KEY, SecurityAlgorithms.HmacSha256);
            JwtSecurityToken jwt = new JwtSecurityToken(signingCredentials: signingCredentials
                                                        , notBefore: DateTime.Now
                                                        , expires: DateTime.Now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME))
                                                        , issuer: AuthOptions.ISSUER
                                                        , audience: AuthOptions.AUDIENCE
                                                        , claims: claimList
                                                        );
            string encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return encodedJwt;
        }

        public static bool IsTokenValid(string token)
        {
            if (string.IsNullOrEmpty(token))
                return false;
            string signature;
            string[] parts = token.Split('.');
            if(parts.Length!=3)
                return false;
            string str = parts[0] + "." + parts[1];
            byte[] bytesToVerify =  Encoding.UTF8.GetBytes(str);
            //string key = 
            using (System.Security.Cryptography.HMACSHA256 hmac = new System.Security.Cryptography.HMACSHA256(Encoding.UTF8.GetBytes(Configuration.GetSection("secretKey").Get<string>())))
            {
                byte[] verifiedBytes = hmac.ComputeHash(bytesToVerify);
                signature = Encoding.UTF8.GetString(verifiedBytes);
                signature = Convert.ToBase64String(verifiedBytes, Base64FormattingOptions.None);
                signature = signature.Replace('+', '-');
                signature = signature.Replace('/', '_');
            }
            if (signature == parts[2] + "=")
                return true;
            else
                return false;
        }

        public static Dictionary<string, string> GetJWTClaimsValues(string token)
        {
            Dictionary<string, string> values = new Dictionary<string, string>();

            JwtSecurityToken jwt = new JwtSecurityToken(token);
            List<Claim> list = jwt.Claims.ToList();
            values.Add("employeeId", list.FirstOrDefault(c => c.Type == "employeeId").Value);
            values.Add("login", list.FirstOrDefault(c => c.Type == "login").Value);
            values.Add("companyId", list.FirstOrDefault(c => c.Type == "companyId").Value);
            values.Add("uerName" , list.FirstOrDefault(c => c.Type == "userName").Value);
            values.Add("roleName", list.FirstOrDefault(c => c.Type == "roleName").Value);

            return values;
        }

            // пробный метод
        public static bool ValidateToken(string authToken)
        {
            JwtSecurityToken jwt = new JwtSecurityToken(authToken);
            var tokenHandler = new JwtSecurityTokenHandler();
            var validationParameters = new TokenValidationParameters()
            {
                ValidateLifetime = false, // Because there is no expiration in the generated token
                ValidateAudience = false, // Because there is no audiance in the generated token
                ValidateIssuer = false,   // Because there is no issuer in the generated token
                ValidIssuer = AuthOptions.ISSUER,
                ValidAudience = AuthOptions.AUDIENCE,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration.GetSection("secretKey").Get<string>()))
            };

            SecurityToken validatedToken;
            ClaimsPrincipal principal = tokenHandler.ValidateToken(authToken, validationParameters, out validatedToken);
            return true;
        }

        /// <summary>
        /// Получение Id компании из токена
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static int GetCompanyIdByJWT(string token)
        {
            try
            {
                JwtSecurityToken jwt = new JwtSecurityToken(token);
                return int.Parse(jwt.Claims.FirstOrDefault(c => c.Type == "companyId").Value);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        /// <summary>
        /// Получение Id юзера из токена
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static int GetUserIdByJWT(string token)
        {
            try
            {
                JwtSecurityToken jwt = new JwtSecurityToken(token);
                return int.Parse(jwt.Claims.FirstOrDefault(c => c.Type == "employeeId").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        public static Guid GetUserUidByJWT(string token)
        {
            try
            {
                JwtSecurityToken jwt = new JwtSecurityToken(token);
                return Guid.Parse(jwt.Claims.FirstOrDefault(c => c.Type == "employeeUid").Value);
            }
            catch (Exception)
            {
                return Guid.Empty;
            }
        }

    }




    static class AuthOptions
    {
        public const string ISSUER = "https://localhost:44332/"; // издатель токена
        public const string AUDIENCE = "https://localhost:44332/"; // потребитель токена
        //const string KEY = "mysupersecret_secretkey!123";   // ключ для шифрации
        public const int LIFETIME = 60; // время жизни токена - 1 минута
        //public static SymmetricSecurityKey GetSymmetricSecurityKey()
        //{
        //    return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        //}
    }
}

