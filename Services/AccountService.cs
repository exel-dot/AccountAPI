using AccountAPI.Entities;
using AccountAPI.Exceptions;
using AccountAPI.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Server.IIS.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileSystemGlobbing.Internal.PatternContexts;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace AccountAPI.Service
{
    
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto dto);
        IEnumerable<UserDto> GetUsers();
        string GenerateJwt(LoginDto dto);
        IEnumerable<UserInformationsdto> Find(AccountQuery query);
    }
    public class AccountService : IAccountService
    {
        private readonly AccountDbContext _dbcontext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountService> _logger;
        private readonly AuthenticationSettings authenticationsettings;
       

        public AccountService(AccountDbContext context, IPasswordHasher<User> passwordHasher, IMapper mapper, ILogger<AccountService> logger, AuthenticationSettings authenticationsettings)
        {
            _dbcontext = context;
            _passwordHasher = passwordHasher;
            this._mapper = mapper;
            this._logger = logger;
            this.authenticationsettings = authenticationsettings;
          
        }
        public void RegisterUser(RegisterUserDto dto)
        {

            var newUser = new User()
            {
                Email = dto.Email,
                DateOfBirth = dto.DateOfBirth,
                Pesel = dto.Pesel,
                Name = dto.Name,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,
                EnergyConsumption = dto.EnergyConsumption,

            };

            if (newUser.EnergyConsumption != null)
            {
                newUser.EnergyConsumption = (float?)Math.Round((decimal)newUser.EnergyConsumption, 3);
            }

            var hashedPassword = _passwordHasher.HashPassword(newUser, dto.Password);
            newUser.Password = hashedPassword;
            _dbcontext.Users.Add(newUser);
            _dbcontext.SaveChanges();
            _logger.LogInformation($"User {newUser.Email} is created");

        }
        public IEnumerable<UserDto> GetUsers()
        {
            var user = _dbcontext.Users.ToList();

            var usersDtos = _mapper.Map<List<UserDto>>(user);
            return usersDtos;
        }
        public IEnumerable<UserInformationsdto> Find(AccountQuery query)
        {

            var userr = _dbcontext.Users.FirstOrDefault(u => u.Email == query.email);
            if (userr == null)
            {
                _logger.LogError("Find action are invoked , Invalid Email  or Password");
                throw new BadRequestException("Invalid Email  or Password");
               

            }
            var result = _passwordHasher.VerifyHashedPassword(userr, userr.Password, query.password);
            if (result == PasswordVerificationResult.Failed)
            {
                _logger.LogError("Find action are invoked , Invalid Email  or Password");
                throw new BadRequestException("Invalid Email  or Password");
                
            }
            else
            {
                var user = _dbcontext.Users

                               .Where(x => x.Email.Equals(query.email)).ToList();

                var usersDtos = _mapper.Map<List<UserInformationsdto>>(user);

                return usersDtos;
            }

        }
        public string GenerateJwt(LoginDto dto)
        {
            var user=_dbcontext.Users.FirstOrDefault(u=>u.Email== dto.Email);

            if (user == null)
            {
                throw new BadRequestException("Invalid Email  or Password");
            }
          var result=  _passwordHasher.VerifyHashedPassword(user, user.Password, dto.Password);
            if (result == PasswordVerificationResult.Failed)
            {
                throw new BadRequestException("Invalid Email  or Password");
            }
            var claims = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                  new Claim(ClaimTypes.Name, $"{user.Name}{user.LastName}"),
                   new Claim("Email", user.Email),

            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationsettings.JwtKey));

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expires = DateTime.Now.AddDays(authenticationsettings.JwtExpireDays);

            var token = new JwtSecurityToken(authenticationsettings.JwtIssuer,
                authenticationsettings.JwtIssuer,
                claims,
                expires: expires,
               signingCredentials: cred);

            var tokenHandler = new JwtSecurityTokenHandler();
            return tokenHandler.WriteToken(token);

        }
    }
}
