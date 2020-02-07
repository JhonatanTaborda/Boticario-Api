using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Security.Claims;
using Boticario.Api.Models;
using Boticario.Api.Repository.Interfaces;
using Boticario.Api.Repository.Validate;

namespace Boticario.Api.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;        
        private readonly IConfiguration _configuration;
        private readonly ApiExtensions _ext;

        public UserRepository(UserManager<ApplicationUser> userManager,            
                IConfiguration configuration,
                ApiExtensions ext)
        {
            _userManager = userManager;            
            _configuration = configuration;
            _ext = ext;
        }

        /// <summary>
        /// Retorna lista de usuários
        /// </summary>
        /// <returns>IList<ApplicationUser></returns>
        public async Task<IList<ApplicationUser>> GetAll()
        {
            return await _userManager.Users.ToListAsync();

        }

        /// <summary>
        /// Retorna o usuários pelo Id
        /// </summary>
        /// <param name="idUser" type="string">Id do usuário</param>
        /// <returns>ApplicationUser</returns>
        public async Task<ApplicationUser> Get(string idUser)
        {
            return await _userManager.FindByIdAsync(idUser);
        }

        /////// <summary>
        /////// Método de atualização de email do usuário
        /////// </summary>
        /////// <param name="userInfo">UserinfoModel</param>
        /////// <returns>IList<ApplicationUser></returns>
        ////public async Task<IdentityResult> ChangeEmail([FromBody] UserInfo userInfo)
        ////{
        ////    var user = await _userManager.FindByIdAsync(userInfo.UserId);
        ////    user.Email = userInfo.Email;

        ////    var tokenEmail = await _userManager.GenerateChangeEmailTokenAsync(user, userInfo.Email);
        ////    //var accessToken = Request.Headers.FirstOrDefault(x => x.Key == "Authorization");

        ////    var result = await _userManager.ChangeEmailAsync(user, userInfo.Email, tokenEmail);

        ////    return result;
        ////}

        /// <summary>
        /// Métdo de criação de usuários
        /// </summary>
        /// <param name="model">model UserInfo</param>
        /// <returns>Retorna o usuário criado</returns>
        public async Task<UserInfo> CreateUser(UserInfo model)
        {
            //validar dados do usuários
            var _val = new UserVal(model, _userManager, _ext);
            await _val.UserValidate();

            if (model.IsValid) {
                
                var user = new ApplicationUser { UserName = model.Name, Email = model.Email, PasswordHash = model.Password };

                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {   
                    model.Id = await _userManager.GetUserIdAsync(user);
                    model.IsValid = true;

                    user.Id = model.Id;
                    await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Revendedor"));
                }
                else
                {
                    return new UserInfo
                    {
                        IsValid = result.Succeeded,
                        Message = result.Errors.ToString()
                    };
                }
            }
            
            return model;            
        }
    }
}
