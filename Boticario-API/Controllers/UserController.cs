using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Boticario.Api.Models;
using Boticario.Api.Repository.Interfaces;

namespace Boticario.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoginRepository _loginRepository;

        public UserController(IUserRepository userRepository,
                            ILoginRepository loginRepository)
        {
            _userRepository = userRepository;
            _loginRepository = loginRepository;
        }
        
        [HttpGet("Get")]
        [AutoValidateAntiforgeryToken]
        public async Task<ActionResult<IList<ApplicationUser>>> GetAll()
        {
            var resutl = await _userRepository.GetAll();

            return Ok(resutl);
        }

        [Authorize]
        [HttpPost("Save")]        
        public async Task<ActionResult<UserInfo>> CreateUser([FromBody] UserInfo model)
        {
            return await _userRepository.CreateUser(model);
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult<UserTokenModel>> Login([FromBody] UserInfo userInfo)
        {
            return await _loginRepository.Auth(userInfo);
        }

        [HttpPost("Logout")]
        public async Task<ActionResult> Logout([FromBody] UserInfo userInfo)
        {
            await _loginRepository.Logout(userInfo);

            return Ok();
        }                   
    }
}