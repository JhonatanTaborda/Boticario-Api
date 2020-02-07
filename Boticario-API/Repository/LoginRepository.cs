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

namespace Boticario.Api.Repository
{
    public class LoginRepository : ILoginRepository
    {        
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApiExtensions _ext;


        /// <summary>
        /// Construtor da Classe
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="configuration"></param>
        public LoginRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApiExtensions ext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _ext = ext;
        }

        /// <summary>
        /// Método de Autenticação
        /// </summary>
        /// <param name="userInfo">Model UserInfo</param>
        /// <returns>UserTokenModel</returns>        
        public async Task<UserTokenModel> Auth(UserInfo userInfo)
        {
            var user = await _userManager.FindByEmailAsync(userInfo.Email);

            if (user != null)
            {
                var password = await _signInManager.PasswordSignInAsync(user, _ext.DecryptHash(userInfo.Password), true, false);

                if (password.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    
                    var result = await _signInManager.CanSignInAsync(user);
                                                 
                    if (result)
                    {
                        var tokenModel = _ext.BuildToken(userInfo);

                        await _userManager.SetAuthenticationTokenAsync(user, "Bearer", user.Id, tokenModel.AccessToken);                                                
                    }

                    return new UserTokenModel { Message = "Erro ao tetar realizar o Login!" };
                }
                else
                {
                    return new UserTokenModel { Message = "Senha inválida" }; 
                }
            }
            else
            {
                return new UserTokenModel { Message = "Email não encontrado!" };               
            }
        }

        /// <summary>
        /// Método de Logout e exclusão to token de autorização
        /// </summary>
        public async Task Logout(UserInfo userInfo)
        {
            await _signInManager.SignOutAsync();
            
            var userApp = new ApplicationUser { Id = userInfo.Id, UserName = userInfo.Name, Email = userInfo.Email };

            await _userManager.RemoveAuthenticationTokenAsync(userApp, "Bearer", userApp.Id);
        }
    }
}
