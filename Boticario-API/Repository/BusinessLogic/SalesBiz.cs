using Boticario.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boticario.Api.Repository.BusinessLogic
{
    /// <summary>
    /// Classe de regras de negócio
    /// </summary>
    public class SalesBiz : BaseBiz<SalesModel>
    {
        public SalesBiz(SalesModel model) : base(model)
        {
            Model = model;
        }

        public void ApplyCacheBack()
        {   
            if (Model.Value <= 1000)
            {
                Model.PercentCacheBack = 10;
            }
            else if (Model.Value > 1000 && Model.Value <= 1500)
            {
                Model.PercentCacheBack = 15;
            }
            else if (Model.Value > 1500)
            {
                Model.PercentCacheBack = 20;
            }
            else
            {
                Model.PercentCacheBack = 0;
            }

            if (Model.PercentCacheBack > 0)
            {
                Model.ValueCacheBack = Model.Value * Convert.ToDecimal(Model.PercentCacheBack / 100.0);
            }
            else
            {
                Model.ValueCacheBack = 0;
            }
        }
    }
}
