using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KalymnosBT.Models;
using KalymnosBT.ViewModels;

namespace KalymnosBT.DesignData
{
    class SampleBackstageViewModel : BackstageViewModel
    {
        public SampleBackstageViewModel()
        {
            Model = KalymnosDbHelper.DefaultDb();
        }
    }
}
