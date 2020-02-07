using Boticario.Api.Models;
using Boticario.Api.Models.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boticario.Api.Repository.Validate
{
    /// <summary>
    /// Classe de validações
    /// </summary>
    public class SalesVal : BaseVal<SalesModel>
    {
        private readonly IConfiguration _configuration;
        private readonly UserManager<ApplicationUser> _userManager;

        public SalesVal(SalesModel model,
                IConfiguration configuration,
                UserManager<ApplicationUser> userManager) : base(model)
        {
            Model = model;
            _configuration = configuration;
            _userManager = userManager;
        }

        public async Task SalesValidate()
        {
            if (Model.Value <= 0)
            {
                Model.IsValid = false;
                Model.Message = "Valor do Pedido inválido";
                return;
            }

            if (string.IsNullOrEmpty(Model.SkuProduct))
            {
                Model.IsValid = false;
                Model.Message = "O código do Produto deve ser informado";
                return;
            }

            if (string.IsNullOrEmpty(Model.Id))
            {
                Model.Status = SalesStatus.EmValidacao;
            }

            if (string.IsNullOrEmpty(Model.IdApplicationUser))
            {   
                Model.IsValid = false;
                Model.Message = "O código do Revendedor deve ser informado";
                return;
            }
            else
            {
                var _user = await _userManager.FindByIdAsync(Model.IdApplicationUser);
                //TODO: Coloquei no appsettings para agilizar o desenvolvimento, mas poderia se no banco 
                if (_configuration["CPF_Aprovado"] == _user.CPF)
                {
                    if (string.IsNullOrEmpty(Model.Id))
                    {
                        Model.Status = SalesStatus.Aprovado;
                        Model.Id = Guid.NewGuid().ToString();
                    }
                }
            }             
        }

        public void ChangeValidate()
        {
            Model.IsValid = true;

            if (Model.Status != SalesStatus.EmValidacao)
            {
                Model.IsValid = false;
                Model.Message = "Esta compra não pode ser excluída ou alterada pois esta no Status de " + Model.Status.ToString();                
            }

            return;
        }
    }
}
