using DAL.DTO.Req;
using DAL.DTO.Res;
using DAL.Models;
using DAL.Repositories.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Services
{
    public class UserServices : IUserServices
    {
        private readonly PeerlandingContext _context;
        private readonly IConfiguration _configuration;
        public UserServices(PeerlandingContext context, IConfiguration configuration) 
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<List<ResUserDto>> GetAllUsers()
        {
            return await _context.MstUsers
                .Where(user => user.Role !="admin")
                .Select(user => new ResUserDto
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Role = user.Role,
                    Balance = user.Balance,
                }).ToListAsync();
        }

        public async Task<ResLoginDto> Login(ReqLoginDto reqLogin)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == reqLogin.Email);
            if (user == null)
            {
                throw new Exception("Invalid email or password");
            }
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(reqLogin.Password, user.Password);
            if (!isPasswordValid)
            {
                throw new Exception("Invalid email or password");
            }
            var id = user.Id;
            var token = GenerateJwtToken(user);
            var loginResponse = new ResLoginDto
            {
                Id = id,
                Token = token,
            };

            return loginResponse;
        }   
        private string GenerateJwtToken(MstUser user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Email),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var token = new JwtSecurityToken(
                issuer: jwtSettings["ValidIssuer"],
                audience: jwtSettings["ValidAudience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<string> Register(ReqRegisterUserDto register)
        {
            var isAnyEmail = await _context.MstUsers.SingleOrDefaultAsync(e => e.Email == register.Email);
            if (isAnyEmail != null)
            {
                throw new Exception("Email already used");
            }

            var newUser = new MstUser
            {
                Name = register.Name,
                Email = register.Email,
                //Password = register.Password,
                Role = register.Role,
                //Balance = register.Balance,
            };

            await _context.MstUsers.AddAsync(newUser);
            await _context.SaveChangesAsync();

            return newUser.Name;
        }

        public async Task<ResUserByIdDto> GetUserById(string id)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new ResErrorDto
                {
                    Data = null,
                    Message = "User not found",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            var result = new ResUserByIdDto
            {
                Id = id,
                Name = user.Name,
                Role = user.Role,
                //Email = user.Email,
                Balance = user.Balance
            };
            return result;
        }

        public async Task DeleteUserById(string id)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new ResErrorDto
                {
                    Message = "User not found",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            await _context.MstUsers.Where(x => x.Id == id).ExecuteDeleteAsync();
        }

        public async Task<ResUserDto> UpdateUserById(string id, ReqUpdateUserDto updateUser)
        {
            var user = await _context.MstUsers.SingleOrDefaultAsync(x => x.Id == id);
            if (user == null)
            {
                throw new ResErrorDto
                {
                    Message = "User not found",
                    StatusCode = StatusCodes.Status404NotFound
                };
            }
            user.Name = updateUser.Name;
            user.Role = updateUser.Role;
            user.Balance = updateUser.Balance;
            await _context.SaveChangesAsync();
            return new ResUserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Role = user.Role,
                Balance = user.Balance
            };
        }
    }
}
