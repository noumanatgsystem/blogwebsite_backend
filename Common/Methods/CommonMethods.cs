using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.User;
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
        public static string GenerateJwtToken(string UserEmail, long UserID, string UserName,string role)
        {

            var authClaims = new List<Claim>
            {
                new Claim("Email", UserEmail),
                new Claim("ID", UserID.ToString()),
                new Claim("UserName", UserName),
                new Claim("Role",role)
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

        #endregion

        #region Email
        public static async Task<string> SendEmail(string userEmail, string subject, string body, IConfiguration config)
        {
            string response = "";

            var smtpSettings = config.GetSection("SmtpSettings");

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
        public static void SendVerificationEmail(AppUser AppUser, IConfiguration _config)
        {

            if (AppUser != null)
            {
                var token = GenerateJwtToken(AppUser.Email, AppUser.Id, AppUser.UserName,AppUser.Role);
                if (string.IsNullOrEmpty(AppUser.Email))
                {
                    return;
                }

                //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "verifyemail.html");
                //string htmlContent = System.IO.File.ReadAllText(filePath);
                string link = $"https://admissionlylo.com/?VerifyToken={token}";
                //htmlContent = htmlContent.Replace("{{VerificationLink}}", link);
                SendEmail(AppUser.Email, "AdmissionLelo Verification Email","Hello", _config);

            }
        }

        #endregion
    }
}
