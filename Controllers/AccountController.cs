using AccountAPI.Models;
using AccountAPI.Service;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AccountAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        public AccountController(IAccountService _accountservice, IMapper mapper)
        {
            _accountService = _accountservice;
            _mapper = mapper;

        }
        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDto dto)
        {
            _accountService.RegisterUser(dto);
            return Ok();
        }
        [HttpGet("users")]
        [Authorize]
        public ActionResult<IEnumerable<UserDto>> GetUsers()
        {
            var userDtos = _accountService.GetUsers();
            return Ok(userDtos);
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDto dto)
        {


            string token = _accountService.GenerateJwt(dto);
            return Ok(token);

        }
        [HttpGet("find")]
        [Authorize]
        public ActionResult<IEnumerable<UserDto>> Find([FromBody] AccountQuery query)
        {
            var UserDtos = _accountService.Find(query);
            return Ok(UserDtos);
        }

    }
}
