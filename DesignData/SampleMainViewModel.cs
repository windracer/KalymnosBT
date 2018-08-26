using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KalymnosBT.Models;
using KalymnosBT.ViewModels;

namespace KalymnosBT.DesignData
{
    public class SampleMainViewModel : MainViewModel
    {
        public SampleMainViewModel()
        {
            Model = KalymnosDbHelper.DefaultDb();
            SelectedProject = Model.Projects.FirstOrDefault(
                p => string.Compare(p.Name, "WinCatalog", StringComparison.InvariantCultureIgnoreCase) == 0);

            SelectedIssue = SelectedProject.Issues.FirstOrDefault(i => i.IssueId == 225);
        }
    }
}
