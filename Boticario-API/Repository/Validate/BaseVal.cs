using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Boticario.Api.Repository.Validate
{
    public class BaseVal<TModel>
    {
        protected TModel _model;

        public TModel Model { get; set; }

        public BaseVal(TModel model)
        {
            _model = model;            
        }
    }
}
