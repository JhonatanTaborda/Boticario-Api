using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boticario.Api.Repository.BusinessLogic
{
    public class BaseBiz<TModel>
    {
        protected TModel _model;

        public TModel Model { get; set; }

        public BaseBiz(TModel model)
        {
            _model = model;
        }
    }
}
