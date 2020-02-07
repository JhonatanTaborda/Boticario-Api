using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Boticario.Api.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Boticario.Api.Repository.Interfaces
{
    public interface ISalesRepository
    {
        Task<SalesModel> Create(SalesModel model);
        Task<SalesModel> Update(SalesModel model);
        Task Delete(string Id);
        Task<IList<SalesModel>> GetAll();
        Task<SalesModel> Get(string id);
        ServiceCashbackModel GetCashbackByUser(string cpf);
    }
}
