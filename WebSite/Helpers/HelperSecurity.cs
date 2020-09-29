using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

using System.Text;
using System.Security.Claims;
using System.Linq;
using Avokates_CRM.Models.Outputs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Cryptography;
using Avokates_CRM.Models.ApiModels;
using Avokates_CRM.Models.DB;

namespace WebSite.Helpers
{
    public static class HelperSecurity
    {
        static IConfiguration Configuration;
        public static void Init(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        #region JWT-токен

        public static string GenerateToken(Authorization_Out_FromDB auth)
        {
            List<Claim> claimList = new List<Claim>();

            //claimList.Add(new Claim("employeeId", auth.EmployeeId.ToString()));
            claimList.Add(new Claim("employeeUid", auth.EmployeeUid.ToString()));
            claimList.Add(new Claim("companyUid", auth.CompanyUID.ToString()));
            //claimList.Add(new Claim("login", auth.Login));
            //claimList.Add(new Claim("companyId", auth.CompanyId.ToString()));

            //claimList.Add(new Claim("userName", string.IsNullOrEmpty(auth.Name)?"": auth.Name));           
            claimList.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, string.IsNullOrEmpty(auth.RoleName) ? "" : auth.RoleName));

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

        public static JWTClaims GetJWTClaimsValues(string token)
        {

            //Dictionary<string, string> values = new Dictionary<string, string>();

            JwtSecurityToken jwt = new JwtSecurityToken(token);
            List<Claim> list = jwt.Claims.ToList();
            JWTClaims values = new JWTClaims()
            {
                companyUid = Guid.Parse(list.FirstOrDefault(c => c.Type == "companyUid").Value),
                employeeUid = Guid.Parse(list.FirstOrDefault(c => c.Type == "employeeUid").Value),
                role = list.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).Value
            };

            //values.Add("employeeId", list.FirstOrDefault(c => c.Type == "employeeId").Value);
            //values.Add("login", list.FirstOrDefault(c => c.Type == "login").Value);
            //values.Add("companyId", list.FirstOrDefault(c => c.Type == "companyId").Value);
            //values.Add("companyUid", list.FirstOrDefault(c => c.Type == "companyUid").Value);
            //values.Add("employeeUid", list.FirstOrDefault(c => c.Type == "employeeUid").Value);
            //values.Add("role", list.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).Value);
            //values.Add("uerName" , list.FirstOrDefault(c => c.Type == "userName").Value);
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

        public static bool IsTokenAdmin(string token)
        {
            try
            {
                JwtSecurityToken jwt = new JwtSecurityToken(token);
                string role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
                return (role == "admin" || role == "director");
            }
            catch
            {
                return false;
            }
        }

        public static bool IsTokenDirector(string token)
        {
            try
            {
                JwtSecurityToken jwt = new JwtSecurityToken(token);
                string role = jwt.Claims.FirstOrDefault(c => c.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
                return (role == "director");
            }
            catch
            {
                return false;
            }
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
        /// Получение UID компании из токена
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Guid GetCompanyUidByJWT(string token)
        {
            try
            {
                JwtSecurityToken jwt = new JwtSecurityToken(token);
                return Guid.Parse(jwt.Claims.FirstOrDefault(c => c.Type == "companyUid").Value);
            }
            catch (Exception ex)
            {
                return Guid.Empty;
            }
        }

        ///// <summary>
        ///// Получение Id юзера из токена
        ///// </summary>
        ///// <param name="token"></param>
        ///// <returns></returns>
        //public static int GetUserIdByJWT(string token)
        //{
        //    try
        //    {
        //        JwtSecurityToken jwt = new JwtSecurityToken(token);
        //        return int.Parse(jwt.Claims.FirstOrDefault(c => c.Type == "employeeId").Value);
        //    }
        //    catch (Exception)
        //    {
        //        return 0;
        //    }
        //}

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

        #endregion

        // Получение информации по юзеру касательно определённого дела
        public static EmployeeCaseInfo GetEmployeeCaseInfo(string token, Guid caseUid, LawyerCRMContext _context)
        {
            JWTClaims JWTValues = HelperSecurity.GetJWTClaimsValues(token);
            Guid userUidFromToken = JWTValues.employeeUid;
            //Guid companyUidFromToken = JWTValues.companyUid;
            EmployeeCaseInfo employeeCaseInfo = _context.EmployeeCase
                                                .Where(e => e.EmployeeUid == userUidFromToken && e.CaseUid == caseUid )
                                                .Select(e => new EmployeeCaseInfo
                                                {
                                                    encriptedAesKey = e.EncriptedAesKey,
                                                    isOwner = e.IsOwner,
                                                    employeeGuid = userUidFromToken
                                                })
                                                .FirstOrDefault();
            employeeCaseInfo.userRole = JWTValues.role;

            return employeeCaseInfo;
        }


        #region Криптография
        //-------------------------------------     Создание ключей шифрования      ------------------------------------

        // создание пары ключей RSA
        public static Tuple<string, byte[]> CreateKeyPair()
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 /* PROV_RSA_FULL */ };
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(2048, cspParams);
            byte[] publicKey = rsaProvider.ExportCspBlob(false);
            string privateKey = Convert.ToBase64String(rsaProvider.ExportCspBlob(true));
            return new Tuple<string, byte[]>(privateKey, publicKey);
        }


        /// <summary>
        /// Создание AES-ключа
        /// </summary>
        /// <param name="publicKey">Публичный ключ RSA</param>
        /// <returns>Item1 - симметричный ключ, зашифрованный публичным, Item1 - симметричный ключ в открытом виде</returns>
        public static Tuple<byte[], byte[]> CreateAESKeyEncryptedByRSA(byte[] publicKey)
        {
            // генерируем симметричный ключ
            Aes aes = Aes.Create();
            byte[] key = aes.Key;
            // шифруем симметричный ключ публичным асимметричным
            byte[] encriptedAesKeyByRSA = EncryptByRSA(publicKey, key);
            return new Tuple<byte[], byte[]>(encriptedAesKeyByRSA, key); 
        }

//---------------------------------------------------------------   Симметричное шифрование     --------------------------------------------------------

        /// <summary>
        /// Шифрование алгоритмом AES
        /// </summary>
        /// <param name="text">Исходный текст</param>
        /// <param name="key">Симметричный ключ</param>
        /// <returns>Шифротекст с вектором инициализации в первых 16 байтах</returns>
        public static byte[] EncryptByAes(string text, byte[] key)
        {
            Aes aes = Aes.Create();
            aes.Key = key;
            aes.GenerateIV();
            ICryptoTransform encryptor = aes.CreateEncryptor();
            byte[] bufer = Encoding.UTF8.GetBytes(text);
            byte[] encriptedText = aes.IV.Concat(encryptor.TransformFinalBlock(bufer, 0, bufer.Length)).ToArray();
            return encriptedText;
        }

        /// <summary>
        /// Расшифровка исходного сообщения, зашифрованного алгоритмом AES
        /// </summary>
        /// <param name="encriptedText">Шифротекст с вектором инициализации в первых 16 байтах</param>
        /// <param name="key">Симметричный ключ</param>
        /// <returns>Исходный текст</returns>
        public static string DecriptByAes(byte[] encriptedText, byte[] key)
        {
            Aes aes = Aes.Create();
            aes.Key = key;
            aes.IV = encriptedText.Take(16).ToArray();

            ICryptoTransform decriptor = aes.CreateDecryptor();
                //  16 байт - вектор инициализации
            byte[] decryptedText = decriptor.TransformFinalBlock(encriptedText, 16, encriptedText.Length - 16);
            string text = Encoding.UTF8.GetString(decryptedText);
            return text;
        }
        //---------------------------------------------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------   Асимметричное шифрование     --------------------------------------------------------

        // шифрование AES ключа публичным RSA
        public static byte[] EncryptByRSA(byte[] publicKey, byte[] data)
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 };
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);
            rsaProvider.ImportCspBlob(publicKey);
            byte[] encryptedBytes = rsaProvider.Encrypt(data, false);
            return encryptedBytes;
        }

        // Расшифровка AES ключа приватным RSA
        public static byte[] DecryptByRSA(string privateKey, byte[] encryptedBytes)
        {
            CspParameters cspParams = new CspParameters { ProviderType = 1 };
            RSACryptoServiceProvider rsaProvider = new RSACryptoServiceProvider(cspParams);
            rsaProvider.ImportCspBlob(Convert.FromBase64String(privateKey));
            byte[] plainBytes = rsaProvider.Decrypt(encryptedBytes, false);
            return plainBytes;
        }

    }

    #endregion


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

    public class JWTClaims
    {
        public Guid companyUid;
        public Guid employeeUid;
        public string role;
    }
}

