using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DataTransferModels.AppUser;
using Application.DataTransferModels.Authentication;
using Application.DataTransferModels.Common;
using Application.DataTransferModels.ResponseModel;
using Application.Interface.User;
using Azure;
using Common.Constants;
using Common.Methods;
using Dapper;
using Domain.Models.User;
using Google.Apis.Auth;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using static Application.DataTransferModels.Authentication.ChangePasswordVm;

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
        public ResponseVm CreateUser(UserVM user)
        {
            ResponseVm response = ResponseVm.Instance;

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
                //CommonMethods.SendVerificationEmail(userModel, _config);
                response.responseCode = ResponseCode.Success;
                response.responseMessage = "Account created successfully.Verify your email";
                return response;

        }
        public ResponseVm VerifyUserEmail(string verifyToken)
        {
            ResponseVm response = ResponseVm.Instance;

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
        public ResponseVm LoginUser(LoginUserVM model)
        {
            ResponseVm response = ResponseVm.Instance;

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
        public ResponseVm ChangeUserPassword(ChangeUserPasswordVm model)
        {
            ResponseVm response = ResponseVm.Instance;

            // Validate if NewPassword matches ConfirmPassword
            if (model.NewPassword != model.ConfirmPassword)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = "New password and confirmation password do not match.";
                return response;
            }

            // Retrieve the user based on the provided UserID
            var user = _context.AppUser.FirstOrDefault(x => x.Id == model.UserID);

            if (user == null)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = "User not found.";
                return response;
            }

            // Validate the old password
            if (user.Password != CommonMethods.EncrypthePassword(model.OldPassword))
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = "Invalid old password.";
                return response;
            }

            // Update the password and save changes
            user.Password = CommonMethods.EncrypthePassword(model.NewPassword);
            _context.SaveChanges();

            response.responseCode = ResponseCode.Success;
            response.responseMessage = "Password changed successfully.";
            return response;
        }
        public ResponseVm RefreshToken(RefreshTokenVm model)
        {
            var response = ResponseVm.Instance;
            var userId = CommonMethods.GetUserIdFromExpiredToken(model.Token);

            if (userId == null)
            {
                response.responseMessage = "Invalid Token";
                response.responseCode = ResponseCode.BadRequest;
                return response;
            }

            var refreshToken = _context.RefreshToken.FirstOrDefault(u => u.UserId == userId);

            if (refreshToken == null || refreshToken.IsExpired)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = "Session Expired Please Login Again";
            }
            else
            {
                var user = _context.AppUser.FirstOrDefault(c => c.Id == userId && !c.IsDeleted && !c.IsBlocked);

                if (user != null)
                {
                    var loginData = new LoggedInUser
                    {
                        Token = CommonMethods.GenerateJwtToken(user.Email, userId.Value, user.UserName,user.Role),
                        RefreshToken = refreshToken.Token,
                        Email = user.Email,
                        UserName = user.UserName,
                        FullName = user.FullName,
                        Role = user.Role,
                        //ProfileBase64 = CommonMethods.RetriveFromWasabi(user.ProfileImageUrl)
                    };

                    response.responseMessage = "Session Updated Succesfully";
                    response.responseCode = ResponseCode.Success;
                    response.data = loginData;
                }
            }

            return response;
        }
        public async Task<ResponseVm> LoginWithGoogle(string idToken,string role)
        {
            ResponseVm response = ResponseVm.Instance;

            var setting = new GoogleJsonWebSignature.ValidationSettings
            {
                Audience = new List<string> { "57908406230-5l08r4mdk26ghr1dj9p6p025q3nb9b2j.apps.googleusercontent.com" }
            };

            // Validate token recived from frontend 
            var res = await GoogleJsonWebSignature.ValidateAsync(idToken, setting);
            if (res == null)
            {
                response.responseCode = ResponseCode.Success;
                response.responseMessage = "UnVerified by Google";
                return response;
            }

            // get exsisting users
            var user = await _context.AppUser.FirstOrDefaultAsync(u => u.Email == res.Email  && !u.IsDeleted && !u.IsBlocked);

            // create new user in db
            if (user == null)
            {
                var newUser = new AppUser
                {
                    FullName = res.Name,
                    Email = res.Email,
                    Password = "",
                    ProfileImageUrl = res.Picture,
                    IsEmailVerified = true,
                    Role = role,
                    UserName = res.Email.Split('@')[0]
                };

                _context.AppUser.Add(newUser);
                await _context.SaveChangesAsync();



                var loginData = new LoggedInUser
                {
                    Token = CommonMethods.GenerateJwtToken(newUser.Email, newUser.Id, newUser.UserName,role),
                    Email = newUser.Email,
                    UserName = newUser.UserName,
                    FullName = newUser.FullName,
                    Role = newUser.Role,
                    //ProfileBase64 = res.Picture
                };

                response.responseCode = ResponseCode.Success;
                response.responseMessage = "Login successfull";
                response.data = loginData;
            }
            else
            {
                // old user 
                var loginData = new LoggedInUser
                {
                    Token = CommonMethods.GenerateJwtToken(user.Email, user.Id, user.UserName,role),
                    Email = user.Email,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Role = user.Role,
                    //ProfileBase64 = res.Picture
                };
                response.responseCode = ResponseCode.Success;
                response.responseMessage = "Login successfull";
                response.data = loginData;
            }



            return response;
        }
        public ResponseVm ResetUserPassword(string resetToken, string password)
        {
            ResponseVm response = ResponseVm.Instance;

            // Extract email from token using the common method
            var email = CommonMethods.ExtractClaimFromToken(resetToken, "Email");
            if (email == null)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = "Invalid or expired reset token";
                return response;
            }

            // Find the user by email and check if they exist
            var user = _context.AppUser.FirstOrDefault(x => x.Email == email && !x.IsDeleted);
            if (user == null)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = "There is no user with this Email";
                return response;
            }

            // Update the user's password
            user.Password = CommonMethods.EncrypthePassword(password);
            _context.Update(user);
            _context.SaveChanges();

            response.responseCode = ResponseCode.Success;
            response.responseMessage = "Password updated successfully";
            return response;
        }
        public ResponseVm ForgetUserPassword(string email)
        {
            ResponseVm response = ResponseVm.Instance;


            var existingMail = _context.AppUser
                                              .FirstOrDefault(e => e.Email == email && !e.IsDeleted);
            if (existingMail == null)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.errorMessage = "No User Found with this Email";
                return response;
            }

            // Check if the user's account is blocked
            if (existingMail.IsBlocked)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = "Please verify your Email first";
                return response;
            }

            try
            {
                // Generate reset token
                var resetToken = CommonMethods.GenerateJwtToken(existingMail.Email, existingMail.Id, existingMail.UserName,existingMail.Role);


                //string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "ResetPassword.html");
                //string htmlContent = System.IO.File.ReadAllText(filePath);
                //string link = $"http://admissionlylo.com/authentication/reset-password?resetToken={resetToken}";
                //htmlContent = htmlContent.Replace("{{resetLink}}", link);
                //CommonMethods.SendEmail(existingMail.UserEmail, "Reset Your Password", htmlContent);

                response.responseCode = ResponseCode.Success;
                response.responseMessage = "Password reset Email sent successfully";
            }
            catch (Exception ex)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = $"Error sending password reset Email: {ex.Message}";
            }

            return response;
        }
        public ResponseVm UnBlockUser(long Id)
        {
            ResponseVm response = ResponseVm.Instance;

            // Check if the user exists and is not deleted
            var user = _context.AppUser.FirstOrDefault(a => a.Id == Id && !a.IsDeleted);

            if (user == null)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.errorMessage = "No active user found";
                return response;  // Return early if user is not found or is deleted
            }

            // Unblock the user
            user.IsBlocked = false;

                // Update the user synchronously and save changes
                _context.AppUser.Update(user);
                _context.SaveChanges();

                response.responseCode = ResponseCode.Success;
                response.responseMessage = "User UnBlocked";

            return response;
        }
        public ResponseVm DeleteUser(long Id)
        {
            ResponseVm response = ResponseVm.Instance;

            // Check if the user exists and is not already deleted
            var user = _context.AppUser.FirstOrDefault(a => a.Id == Id && !a.IsDeleted);

            if (user == null)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.errorMessage = "No active user found";
                return response;  // Return early if user is not found or is deleted
            }

                // Soft delete the user (mark as deleted)
                user.IsDeleted = true;
                _context.AppUser.Update(user);
                _context.SaveChanges();

                response.responseCode = ResponseCode.Success;
                response.responseMessage = "User deleted successfully";

            return response;
        }
        public ResponseVm BlockUser(DeclineVm model)
        {
            ResponseVm response = ResponseVm.Instance;

            // Fetch user by ID, and check if user exists in one go
            var user = _context.AppUser.FirstOrDefault(a => a.Id == model.Id && !a.IsDeleted);
            if (user == null)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.errorMessage = "No active user found with the provided ID.";
                return response;
            }

            // Block the user
            user.IsBlocked = true;
            user.BlockedReason = model.Reason;

            // Update the user and save changes
            _context.AppUser.Update(user);
            _context.SaveChanges();

            response.responseCode = ResponseCode.Success;
            response.responseMessage = "User successfully blocked.";
            return response;
        }
        public ResponseVm ResendVerificationEmail(string email)
        {
            ResponseVm response = ResponseVm.Instance;
            var user = _context.AppUser.FirstOrDefault(op => op.Email == email && !op.IsDeleted);

            if (user == null)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = "User Not Found";
                return response;
            }

            if (user.IsEmailVerified)
            {
                response.responseCode = ResponseCode.BadRequest;
                response.responseMessage = "Email Already Verified";
                return response;
            }

            //CommonMethods.SendVerificationEmail(user, _config);

            response.responseCode = ResponseCode.Success;
            response.responseMessage = "Email Resent! Successfully";
            return response;
        }


        #region Admin 
        public ResponseVm GetUsers(UserSearchVm model)
        {
            ResponseVm response = ResponseVm.Instance;

            var parameters = new DynamicParameters();
            parameters.Add("@SearchText", model.SearchText);
            parameters.Add("@Gender", model.Gender);
            parameters.Add("@UserType", model.UserType);
            parameters.Add("@PageNumber", model.PageNumber);
            parameters.Add("@PageSize", model.PageSize);

            // Retrieve the user data directly as a list of GetUserVm objects
            //var userVms = CommonMethods.ExecuteStoredProcedureAndMaptoModel<GetUserVm>($"SP_Get{model.GetName}Users", parameters);

            //if (userVms == null || !userVms.Any())
            //{
            //    response.responseCode = ResponseCode.Success;
            //    response.responseMessage = "No users found.";
            //    response.data = new List<GetUserVm>();
            //    return response;
            //}

            //var userList = new List<GetUserVm>();

            //foreach (var user in userVms)
            //{
                //if (!string.IsNullOrEmpty(user.ProfileImageUrl))
                //{
                //    user.ProfileImageUrl = CommonMethod.RetriveFromWasabi(user.ProfileImageUrl);
                //}
                //userList.Add(user);
            //}

            //response.data = userList;
            response.responseCode = ResponseCode.Success;
            response.responseMessage = $"Showing results for {model.SearchText}";
            return response;
        }

        #endregion

    }
}
