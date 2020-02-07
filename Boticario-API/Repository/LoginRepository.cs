using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Boticario.Api.Models;
using Boticario.Api.Repository.Interfaces;
using Boticario.Api.Extensions;

namespace Boticario.Api.Repository
{
    public class LoginRepository : ILoginRepository
    {        
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApiExtensions _ext;


        /// <summary>
        /// Construtor da Classe
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="configuration"></param>
        public LoginRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _ext = new ApiExtensions(_configuration);
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
                    try
                    {
                        await _signInManager.SignInAsync(user, isPersistent: true);

                        var result = await _signInManager.CanSignInAsync(user);

                        if (result)
                        {
                            var tokenModel = _ext.BuildToken(userInfo);

                            await _userManager.SetAuthenticationTokenAsync(user, "Bearer", user.Id, tokenModel.AccessToken);
                                                                    
                            return tokenModel;
                        }

                        return new UserTokenModel { Message = "Não foi possível realizar o login, tente novamente por favor!" };
                    }
                    catch (Exception)
                    {                                                                             
                        return new UserTokenModel { Message = "Erro ao tetar realizar o Login!" };
                    }                   
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
            
            var userApp = new ApplicationUser { Id = userInfo.Id, UserName = userInfo.Name, Email = userInfo.Email, SecurityStamp = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6InRlc3RlQGVtYWlsLmNvbSIsImh0dHA6Ly9zY2hlbWFzLnhtbHNvYXAub3JnL3dzLzIwMDUvMDUvaWRlbnRpdHkvY2xhaW1zL2VtYWlsYWRkcmVzcyI6InRlc3RlQGVtYWlsLmNvbSIsImp0aSI6IjVmYTA2NmQ1LTExMzgtNDc0YS04Yjg4LWIyODM3NDA1NmQwMyIsImV4cCI6MTU4MTA5ODIzM30.Ux3pmYWzXvbmf0o0LXCHnTcnKkADqr65hcggKeNS4CI" };

            await _userManager.RemoveAuthenticationTokenAsync(userApp, "Bearer", userApp.Id);
        }
    }
}
