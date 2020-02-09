using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;
using Boticario.Api.Models;
using Boticario.Api.Repository.Interfaces;
using Boticario.Api.Extensions;
using Microsoft.Extensions.Logging;

namespace Boticario.Api.Repository
{
    public class LoginRepository : ILoginRepository
    {        
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ApiExtensions _ext;
        private ILogger _logger;

        /// <summary>
        /// Construtor da Classe
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="signInManager"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public LoginRepository(UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration,
            ILogger<LoginRepository> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _logger = logger;
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

                            _logger.LogInformation(1002, "Usuário " + user.UserName + "|" + user.CPF + " autenticado no sistema.");

                            return tokenModel;
                        }

                        _logger.LogWarning(1002, "Usuário " + user.UserName + "|" + user.CPF + " não autenticado no sistema.");
                        return new UserTokenModel { Message = "Não foi possível realizar o login, tente novamente por favor!" };
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(1002, ex, "LoginController - UserId=" + user.Id + " Message:" + ex.Message);
                        return new UserTokenModel { Message = "Erro ao tetar realizar o Login!" };
                    }                   
                }
                else
                {
                    _logger.LogInformation(1002, "Usuário " + user.UserName + "|" + userInfo.CPF + " informou senha errada.");
                    return new UserTokenModel { Message = "Senha inválida" }; 
                }
            }
            else
            {
                _logger.LogInformation(1002, "Email " + userInfo.Email + " não encontrado.");
                return new UserTokenModel { Message = "Email não encontrado!" };               
            }
        }

        /// <summary>
        /// Método de Logout e exclusão to token de autorização
        /// </summary>
        public async Task Logout(UserInfo userInfo)
        {
            await _signInManager.SignOutAsync();
            
            var userApp = new ApplicationUser { Id = userInfo.Id, UserName = userInfo.Name, Email = userInfo.Email, SecurityStamp = userInfo.Id };

            await _userManager.RemoveAuthenticationTokenAsync(userApp, "Bearer", userApp.Id);

            _logger.LogInformation(1002, "Usuário " + userInfo.Name + "|" + userInfo.CPF + " desconectou do sistema.");
        }
    }
}
