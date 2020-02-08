using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Boticario.Api.Context;
using Boticario.Api.Models;
using Boticario.Api.Repository.BusinessLogic;
using Boticario.Api.Repository.Interfaces;
using Boticario.Api.Repository.Validate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Boticario.Api.Repository
{
    public class SalesRepository : ISalesRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;

        private static string _urlService;
        private static string _tokenService;

        /// <summary>
        /// Construtor da classe
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="configuration"></param>
        /// <param name="logger"></param>
        public SalesRepository(UserManager<ApplicationUser> userManager,
                IConfiguration configuration,
                ILogger<SalesRepository> logger)
        {
            _userManager = userManager;
            _configuration = configuration;
            _logger = logger;
        }

        /// <summary>
        /// Método para criar uma Venda
        /// </summary>
        /// <param name="model">SalesModel</param>
        /// <returns>Retorna o objecto salvo</returns>
        public async Task<SalesModel> Create(SalesModel model)
        {
            //validar dados da compra
            var _val = new SalesVal(model, _configuration, _userManager);
            await _val.SalesValidate();

            _logger.LogInformation("Criar Compra " + model.SkuProduct + "|" + model.Date + "|" + model.IdApplicationUser +", isValid=" + model.IsValid.ToString() + (model.IsValid ? "" : " - Erro: " + model.Message));

            if (model.IsValid)
            {
                var _biz = new SalesBiz(model);
                _biz.ApplyCacheBack();
                
                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();                
                using (var context = new AppDbContext(optionsBuilder.Options))
                {
                    context.Sales.Add(model);

                    context.SaveChanges();                    
                }

                _logger.LogInformation("Compra " + model.SkuProduct + "|" + model.Date + "|" + model.IdApplicationUser + ", criada com id:'" + model.Id + "'");
            }
            
            return model;
        }

        /// <summary>
        /// Método para alterar uma Venda
        /// </summary>
        /// <param name="model">SalesModel</param>
        /// <returns>Retorna o objecto salvo</returns>
        public async Task<SalesModel> Update(SalesModel model)
        {
            //validar dados do usuários
            var _val = new SalesVal(model, _configuration, _userManager);
            _val.ChangeValidate();

            _logger.LogInformation("Alterar Compra id:'" + model.Id + "', isValid=" + model.IsValid.ToString() + (model.IsValid ? "" : " - Erro: " + model.Message));

            if (model.IsValid)
            {
                await _val.SalesValidate();

                _logger.LogInformation("Alterar Compra id:'" + model.Id + "', isValid=" + model.IsValid.ToString() + (model.IsValid ? "" : " - Erro: " + model.Message));
            }

            if (model.IsValid)
            {
                var _biz = new SalesBiz(model);
                _biz.ApplyCacheBack();

                var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
                using (var context = new AppDbContext(optionsBuilder.Options))
                {
                    context.Sales.Update(model);

                    context.SaveChanges();
                }
                
                _logger.LogInformation("Compra id:'" + model.Id + "|" + model.SkuProduct + "|" + model.Date + "|" + model.IdApplicationUser + ", alterada.");
            }

            return model;
        }

        /// <summary>
        /// Método para Excluir uma Venda
        /// </summary>
        /// <param name="Id">id da compra</param>
        /// <returns></returns>
        public async Task Delete(string Id)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                var model = await context.Sales.FirstOrDefaultAsync(x => x.Id == Id);

                if (model != null)
                {
                    var _val = new SalesVal(model, _configuration, _userManager);
                    _val.ChangeValidate();

                    _logger.LogInformation("Excluir Compra id:'" + model.Id + "', isValid=" + model.IsValid.ToString() + (model.IsValid ? "" : " - Erro: " + model.Message));

                    if (model.IsValid)
                    {
                        context.Remove(model);
                        context.SaveChanges();

                        _logger.LogInformation("Excluir Compra id:'" + model.Id + "', excluída.");
                    }                    
                }
                
            }             
        }

        /// <summary>
        /// Retorna a lista de todas as vendas
        /// </summary>
        /// <returns>IList<SalesModel></returns>
        public async Task<IList<SalesModel>> GetAll()
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                return await context.Sales.ToListAsync();                
            }
        }

        /// <summary>
        /// Retorna uma compra pelo Id
        /// </summary>
        /// <param name="id" type="string">Id da compra</param>
        /// <returns>SalesModel</returns>
        public async Task<SalesModel> Get(string id)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                return await context.Sales.FirstOrDefaultAsync(x => x.Id == id);
            }
            
        }

        /// <summary>
        /// Busca o cashback acumulado de um usuário de um serviço externo
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns></returns>
        public ServiceCashbackModel GetCashbackByUser(string cpf)
        {
            _urlService = _configuration["ServiceCashback:url"];
            _tokenService = _configuration["ServiceCashback:token"];

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(
                    new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("token", _tokenService);

                HttpResponseMessage response = client.GetAsync(_urlService + cpf).Result;

                string conteudo = response.Content.ReadAsStringAsync().Result;

                _logger.LogInformation("Consulta de cashback realizada para Cpf:" + cpf + ", result:" + conteudo);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<ServiceCashbackModel>(conteudo);                    
                }

                return new ServiceCashbackModel 
                { 
                    StatusCode = HttpStatusCode.NotFound.GetHashCode(),
                    Body = new BodyCashback { Credit = 0 }
                };
            }
        }
    }
}
