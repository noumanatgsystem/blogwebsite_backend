using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using Domain.Models.User;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Common.Methods
{
    public static class CommonMethods
    {
        public static IConfiguration _config;
        public static void Initialize(IConfiguration configuration)
        {
            _config = configuration;
        }

        #region Authentication
        public static System.String ConvertStringToShah256(string value)
        {
            StringBuilder Sb = new StringBuilder();
            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }
            return Sb.ToString();
        }
        public static string EncrypthePassword(string Password)
        {
            string password = Base64Encode(Password);
            return ConvertStringToShah256(password);
        }
        public static string Base64Encode(string password)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(password);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string GenerateJwtToken(string UserEmail, long UserID, string UserName, string Role)
        {

            var authClaims = new List<Claim>
            {
                new Claim("Email", UserEmail),
                new Claim("ID", UserID.ToString()),
                new Claim("UserName", UserName),
                new Claim("Role",Role)
            };
            var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:Secret"]));
            var token = new JwtSecurityToken(
                issuer: _config["JWT:ValidIssuer"],
                audience: _config["JWT:ValidAudience"],
                expires: DateTime.UtcNow.AddDays(5),
                claims: authClaims,
                signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public static string ExtractClaimFromToken(string token, string claimType)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jwtToken = handler.ReadJwtToken(token);
                var claim = jwtToken.Claims.FirstOrDefault(c => c.Type == claimType);
                return claim?.Value;
            }
            catch
            {
                return null;
            }
        }
        public static RefreshToken GenerateRefreshToken(long userId)
        {
            return new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.UtcNow.AddDays(7),
                UserId = userId
            };
        }
        public static long? GetUserIdFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // Extract user ID from the custom "ID" claim
                var userIdClaim = jwtToken?.Claims?.FirstOrDefault(c => c.Type == "ID");

                if (userIdClaim != null && long.TryParse(userIdClaim.Value, out long userId))
                {
                    return userId;
                }

                return null;
            }
            catch (Exception)
            {
                return null;
            }
        }

        #endregion

        #region Email
        public static async Task<string> SendEmail(string userEmail, string subject, string body)
        {
            string response = "";

            var smtpSettings = _config.GetSection("SmtpSettings");

            using (var client = new SmtpClient(smtpSettings["SmtpServer"], int.Parse(smtpSettings["SmtpPort"])))
            {
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(smtpSettings["SmtpUsername"], smtpSettings["SmtpPassword"]);
                client.EnableSsl = true;

                using (var mailMessage = new MailMessage())
                {
                    mailMessage.From = new MailAddress(smtpSettings["SmtpUsername"]);
                    mailMessage.To.Add(userEmail);
                    mailMessage.Subject = subject;
                    mailMessage.Body = body;
                    mailMessage.IsBodyHtml = true;

                    try
                    {
                        await client.SendMailAsync(mailMessage);
                        response = "Mail sent successfully";
                    }
                    catch (SmtpException smtpEx)
                    {
                        response = $"SMTP error: {smtpEx.Message}";
                    }
                    catch (Exception ex)
                    {
                        response = $"An error occurred: {ex.Message}";
                    }
                }
            }

            return response;
        }
        public static void SendVerificationEmail(AppUser AppUser)
        {

            if (AppUser != null)
            {
                var token = GenerateJwtToken(AppUser.Email, AppUser.Id, AppUser.UserName, AppUser.Role);
                if (string.IsNullOrEmpty(AppUser.Email))
                {
                    return;
                }

                //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "verifyemail.html");
                //string htmlContent = System.IO.File.ReadAllText(filePath);
                //string link = $"https://admissionlylo.com/?VerifyToken={token}";
                //htmlContent = htmlContent.Replace("{{VerificationLink}}", link);
                //SendEmail(AppUser.Email, "AdmissionLelo Verification Email","Hello", _config);

            }
        }

        #endregion

        #region connectionStrings
        public static string GetConnectionString()
        {
            return _config.GetConnectionString("ConnectionString");
        }

        #endregion


        #region DB
        public static List<dynamic> ExecuteStoredProcedure(DynamicParameters parameters, string storedProcedureName)
        {
            string connectionString = GetConnectionString();
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var data = (connection.Query<dynamic>(
                    storedProcedureName,
                    parameters,
                    commandType: CommandType.StoredProcedure
                )).ToList();

                return data;
            }
        }
        public static List<TModel> ExecuteStoredProcedureAndMaptoModel<TModel>(string storedProcedureName, object parameters = null)
        {
            using (IDbConnection dbConnection = new SqlConnection(GetConnectionString()))
            {
                dbConnection.Open();
                var result = dbConnection.QueryMultiple(storedProcedureName, parameters, commandType: CommandType.StoredProcedure);
                var models = result.Read<TModel>();
                return models.ToList();
            }
        }
        #endregion
    }
}
