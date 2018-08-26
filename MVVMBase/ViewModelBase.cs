using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KalymnosBT.MVVMBase
{
    public abstract class ViewModelBase : ObservableObject
    {
    }

    public abstract class ViewModelBase<TModel> : ViewModelBase
    {
        private TModel _model;

        public TModel Model
        {
            get { return _model; }
            set { SetProperty(ref _model, value, () => Model); }
        }
    }
}
