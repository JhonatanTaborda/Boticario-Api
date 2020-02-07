using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
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

        [Authorize]
        [HttpGet]        
        public async Task<ActionResult<IList<ApplicationUser>>> GetAll()
        {
            var resutl = await _userRepository.GetAll();

            return Ok(resutl);
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<IList<ApplicationUser>>> Get([FromRoute] string id)
        {
            var resutl = await _userRepository.Get(id);

            return Ok(resutl);
        }

        [HttpPost("Save")]        
        public async Task<ActionResult<UserInfo>> CreateUser([FromBody] UserInfo model)
        {
            var userReturn = await _userRepository.CreateUser(model);

            if (userReturn.IsValid)
            {
                return Ok(userReturn);
            }
            
            return BadRequest(userReturn);
        }

        [HttpPost("Login")]
        [AllowAnonymous]        
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
        
        [HttpPost("EncryptPassword")]
        [AllowAnonymous]
        public ActionResult<string> EncryptPassword([FromQuery] string password)
        {
            return _userRepository.EncryptPassword(password);
        }
    }
}