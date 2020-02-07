using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Boticario.Api.Models;
using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace Boticario.Api.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task<IList<ApplicationUser>> GetAll();   
        Task<ApplicationUser> Get(string idUser);
        Task<UserInfo> CreateUser([FromBody] UserInfo model);
        string EncryptPassword(string input);
    }
}
