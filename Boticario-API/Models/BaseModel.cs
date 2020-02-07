using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Boticario.Api.Models
{
    public abstract class BaseModel
    {
        public string Id { get; set; }
        [NotMapped]
        public bool IsValid { get; set; }
        [NotMapped]
        public string Message { get; set; }
    }
}
