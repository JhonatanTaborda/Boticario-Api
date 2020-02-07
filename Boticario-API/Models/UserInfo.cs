using Microsoft.EntityFrameworkCore.Metadata;

namespace Boticario.Api.Models
{
    public class UserInfo : BaseModel
    {           
        public string Name { get; set; }
        public string CPF { get; set; }
        public string Email { get; set; }        
        public string Password { get; set; }
    }
}
