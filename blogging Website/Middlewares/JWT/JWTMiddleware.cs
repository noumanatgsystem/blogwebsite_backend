﻿using System.IdentityModel.Tokens.Jwt;
using Application.DataTransferModels.Authentication;
using Microsoft.IdentityModel.Tokens;

namespace blogging_Website.Middlewares.JWT
{
    public class JWTMiddleware :IMiddleware
    {
        private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;
        private readonly TokenValidationParameters _tokenValidationParameters;

        public JWTMiddleware(TokenValidationParameters tokenValidationParameters)
        {
            _jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            _tokenValidationParameters = tokenValidationParameters;
        }

        public async Task InvokeAsync(HttpContext context, RequestDelegate next)
        {
            var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            if (!string.IsNullOrEmpty(token))
            {
                var claimsPrincipal = _jwtSecurityTokenHandler.ValidateToken(token, _tokenValidationParameters, out SecurityToken validatedToken);
                string? email = claimsPrincipal.FindFirst("Email")?.Value;
                string? id = claimsPrincipal.FindFirst("ID")?.Value;
                string? username = claimsPrincipal.FindFirst("UserName")?.Value;
                string? role = claimsPrincipal.FindFirst("Role")?.Value;
                TokenVm toekn = new TokenVm();
                TokenVm.UserEmail = email;
                TokenVm.UserName = username;
                TokenVm.Role = role;
                if (long.TryParse(id, out long userId))
                {
                    TokenVm.UserID = userId;
                }
                context.Items["UserEmail"] = email;
                context.Items["UserID"] = userId;
                context.Items["UserName"] = username;
                context.Items["Role"] = role;

            }
            await next(context);
        }
    }
}
