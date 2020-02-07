using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Boticario.Api.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string CPF { get; set; }
    }
}
