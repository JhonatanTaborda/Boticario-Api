using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Boticario.Api.Models;

namespace Boticario.Api.Repository.Interfaces
{
    public interface ILoginRepository
    {
        Task<UserTokenModel> Auth(UserInfo userInfo);

        Task Logout(UserInfo userInfo);
    }
}
