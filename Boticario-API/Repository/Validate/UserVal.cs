using Boticario.Api.Models;
using Boticario.Api.Repository.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Boticario.Api.Repository.Validate
{
    /// <summary>
    /// Classe de validações
    /// </summary>
    public class UserVal : BaseVal<UserInfo>
    {
        private readonly UserManager<ApplicationUser> _userManager;        
        private readonly ApiExtensions _ext;

        /// <summary>
        /// Construtor da classe
        /// </summary>
        /// <param name="model"></param>
        public UserVal(UserInfo model,
            UserManager<ApplicationUser> userManager,
            ApiExtensions ext) : base(model)
        {               
            Model = model;
            _userManager = userManager;
            _ext = ext;
        }

        /// <summary>
        /// Classe de validação dos dados cadastrais do usuário
        /// </summary>
        /// <returns></returns>
        public async Task UserValidate()
        {
            Model.IsValid = true;

            if(Regex.IsMatch(Model.Email, @"^([\w-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([\w-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$"))
            {
                Model.IsValid = false;
                Model.Message = "Formato de Email inválido"; 
                return;
            }

            var email = await _userManager.FindByEmailAsync(Model.Email);

            if(email != null)
            {
                Model.IsValid = false;
                Model.Message = "Email já cadastrado";      
                return;
            }

            var cpf = _userManager.Users.FirstOrDefault(x => x.CPF == Model.CPF);

            if(cpf != null)
            {
                Model.IsValid = false;
                Model.Message = "CPF já cadastrado";        
                return;
            }

            // valida se tem nome e sobrenome
            if (Model.Name.Trim().Split("\\s+").ToList<string>().Count > 1)
            {
                Model.IsValid = false;
                Model.Message = "Informar Nome e Sobrenome";
                return;
            }

            // valida critérios da senha
            List<string> passwordErrors = new List<string>();

            var validators = _userManager.PasswordValidators;

            var password = _ext.DecryptHash(Model.Password);

            foreach (var validator in validators)
            {
                var result = await validator.ValidateAsync(_userManager, null, password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        passwordErrors.Add(error.Description);
                    }
                }
            }

            if (passwordErrors.Any())
            {
                Model.IsValid = false;
                Model.Message = "Sua senha deve seguir os seguintes critérios: \nDeve conter 8 caracteres com ao menos 1 letra maisculas ou minuscula e 1 ou mais números";
                return;
            }
            else
            {
                Model.Password = password;
            }

            return;
        }
    }
}
