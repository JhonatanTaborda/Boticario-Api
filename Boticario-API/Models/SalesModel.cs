using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Boticario.Api.Models.Enums;

namespace Boticario.Api.Models
{
    [Table("Sales")]
    public class SalesModel : BaseModel
    {
        public DateTime Date { get; set; }
        public string SkuProduct { get; set; }
        public decimal Value { get; set; }
        public int PercentCacheBack { get; set; }
        public decimal ValueCacheBack { get; set; }                
        public SalesStatus Status { get; set; }
        public string IdApplicationUser { get; set;}        
        [ForeignKey("IdApplicationUser")]
        public ApplicationUser Revendedor { get; set; }
    }
}
