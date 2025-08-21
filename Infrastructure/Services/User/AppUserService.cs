using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DataTransferModels.AppUser;
using Application.DataTransferModels.Common;
using Application.Interface.User;
using Azure;
using Common.Constants;
using Common.Methods;
using Domain.Models.User;
using Infrastructure.Context;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Services.User
{
    public class AppUserService: IAppUser
    {
        private readonly IConfiguration _config;
        private readonly AppDbContext _context;

        public AppUserService(IConfiguration config, AppDbContext context)
        {
            _config = config;
            _context = context;
        }
        public ResponseVM CreateUser(UserVM user)
        {
            ResponseVM response = ResponseVM.Instance;

                var existingUser = _context.AppUser
                    .FirstOrDefault(x => (x.Email == user.Email || x.UserName == user.UserName) && !x.IsDeleted);

                if (existingUser != null)
                {
                    response.responseCode = ResponseCode.BadRequest;
                    response.errorMessage = existingUser.Email == user.Email
                        ? "User Already Exists with this Email"
                        : "Username Already Exists";
                    return response;
                }

                var userModel = new AppUser
                {
                    Email = user.Email,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Role = user.Role,
                    Password = CommonMethods.EncrypthePassword(user.Password),
                };

                _context.AppUser.Add(userModel);
                _context.SaveChanges();
                CommonMethods.SendVerificationEmail(userModel, _config);
                response.responseCode = ResponseCode.Success;
                response.responseMessage = "Account created successfully.Verify your email";
                return response;

        }
        public ResponseVM VerifyUserEmail(string verifyToken)
        {
            ResponseVM response = ResponseVM.Instance;

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(verifyToken);

            var email = jwtToken.Claims.FirstOrDefault(claim => claim.Type == "Email")?.Value;

            if (string.IsNullOrEmpty(email))
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = "Invalid token";
                return response;
            }

            var user = _context.AppUser.FirstOrDefault(x => x.Email == email);

            if (user == null)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = "There is no user with this email.";
                return response;
            }

            user.IsEmailVerified = true;
            _context.SaveChanges();
            response.responseCode = ResponseCode.Success;
            response.responseMessage = "Email verified successfully.";

            return response;
        }
        public ResponseVM LoginUser(LoginUserVM model)
        {
            ResponseVM response = ResponseVM.Instance;

            var user = _context.AppUser
                .FirstOrDefault(x => (x.Email == model.Mail || x.UserName == model.Mail) && !x.IsDeleted);

            if (user == null)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = "There is no user with this Email or Username, or the account is deleted.";
                return response;
            }
            if (!user.IsEmailVerified)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = "Please verify your Email first.";
                return response;
            }
            if (user.IsBlocked)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = "Account is Blocked";
                return response;
            }
            if (user.Password != CommonMethods.EncrypthePassword(model.Password))
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = "Invalid password.";
                return response;
            }
            RefreshToken refreshToken = CommonMethods.GenerateRefreshToken(user.Id);

            var oldRefreshToken = _context.RefreshToken.FirstOrDefault(u => u.UserId == user.Id);
            if (oldRefreshToken != null)
            {
                oldRefreshToken.Expires = refreshToken.Expires;
                oldRefreshToken.Token = refreshToken.Token;
                _context.RefreshToken.Update(oldRefreshToken);

            }
            else
            {
                _context.RefreshToken.Add(refreshToken);
            }


            var loginData = new LoggedInUser
            {
                Token = CommonMethods.GenerateJwtToken(user.Email, user.Id, user.UserName,user.Role),
                RefreshToken = refreshToken.Token,
                Email = user.Email,
                UserName = user.UserName,
                FullName = user.FullName,
                Role = user.Role,
                //ProfileImageUrl = CommonMethod.RetriveFromWasabi(user.ProfileImageUrl)
            };

            _context.SaveChanges();
            response.data = loginData;
            response.responseCode = ResponseCode.Success;
            response.responseMessage = "Login successful.";

            return response;
        }

    }
}
