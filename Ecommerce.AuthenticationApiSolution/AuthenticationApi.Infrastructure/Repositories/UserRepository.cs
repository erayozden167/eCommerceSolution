﻿using AuthenticationApi.Application.DTOs;
using AuthenticationApi.Application.Interfaces;
using AuthenticationApi.Domain.Entities;
using AuthenticationApi.Infrastructure.Data;
using BCrypt.Net;
using eCommerce.SharedLibrary.Responses;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AuthenticationApi.Infrastructure.Repositories
{
    public class UserRepository(AuthenticationDbContext context, IConfiguration config) : IUser
    {
        private async Task<AppUserEntity> GetUserByEmail(string email)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Email == email);
            return user is null ? null! : user;
        }
        public async Task<GetUserDTO> GetUser(int userId)
        {
                var user = await context.Users.FindAsync(userId);
                return user is not null ? new GetUserDTO(user.Id,user.Name!,user.TelephoneNumber!,user.Address!,user.Email!,user.Role!) : null!;
        }
        public async Task<Response> Login(LoginDTO loginDTO)
        {
            var getUser = await GetUserByEmail(loginDTO.Email);
            if (getUser is null)
                return new Response(false, "Invalid credentials");
            bool verifyPassword = BCrypt.Net.BCrypt.Verify(loginDTO.Password,getUser.Password);
            if (!verifyPassword)
                return new Response(false, "Invalid credentials");
            string token = GenerateToken(getUser);
            return new Response(true, token);
        }
        private  string GenerateToken(AppUserEntity appUser)
        {
            var key = Encoding.UTF8.GetBytes(config.GetSection("Authentication:Key").Value!);
            var securityKey = new SymmetricSecurityKey(key);
            var credentials = new SigningCredentials(securityKey,SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new (ClaimTypes.Name, appUser.Name!),
                new (ClaimTypes.Email, appUser.Email!),
            };
            if (!string.IsNullOrEmpty(appUser.Role) || !Equals("string", appUser.Role))
                claims.Add(new(ClaimTypes.Role, appUser.Role!));
            var token = new JwtSecurityToken(
                issuer: config["Authentication:Issuer"],
                audience: config["Authentication:Audience"],
                claims: claims,
                expires: null,
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public async Task<Response> Register(AppUserDTO appUserDTO)
        {
            var getUser = await GetUserByEmail(appUserDTO.Email);
            if (getUser is not null)
                return new Response(false, $"User already exist");

            var result = context.Users.Add(new AppUserEntity()
            {
                Name = appUserDTO.Name,
                Email = appUserDTO.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(appUserDTO.Password),
                TelephoneNumber = appUserDTO.TelephoneNumber,
                Address = appUserDTO.Address,
                Role = appUserDTO.Role
            });
            await context.SaveChangesAsync();
            return result.Entity.Id > 0 ? new Response(true, "User successfully added") : new Response(false,"Invalid data provided");
        }
    }
}
