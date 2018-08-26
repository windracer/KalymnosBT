using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KalymnosBT.Models;
using KalymnosBT.ViewModels;

namespace KalymnosBT.DesignData
{
    public class SampleIssueDetailsViewModel : IssueDetailsViewModel
    {
        public SampleIssueDetailsViewModel() : base(new Issue
        {
            IssueId = 42,
            Project = new Project { Prefix = "WCAT"},
            Title = "Fix some bugs",
            Details = "There are some bugs to fix",
            Created = DateTime.Today,
            Modified = DateTime.Now,
            Fixed = DateTime.MaxValue,
            Status = IssueStatus.Closed,
            Comments = { new Comment{ Date = DateTime.Today, Text = "You should do it"}},
            Votes = { new Vote{Name = "M.R.", Email = "mrusakov@gmail.com", When = DateTime.Today}}
        })
        {
        }
    }
}
