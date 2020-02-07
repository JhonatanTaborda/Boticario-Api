using System;

namespace Boticario.Api.Models
{
    public class UserTokenModel : BaseModel
    {
        public string AccessToken { get; set; }
        public DateTime Expiration { get; set; }
        public bool Authenticated { get; set; }        
    }
}
