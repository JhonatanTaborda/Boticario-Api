using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Boticario.Api.Models.Enums
{
    public enum SalesStatus
    {
        [Description("Em Validação")]
        EmValidacao = 0,

        [Description("Aprovado")]
        Aprovado = 1,

        [Description("Excluido")]
        Excluido = 2
    }
}
