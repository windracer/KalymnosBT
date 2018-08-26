using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using KalymnosBT.Models;
using KalymnosBT.MVVMBase;

namespace KalymnosBT.ViewModels
{
    class VoteEditViewModel : ViewModelBase<Vote>
    {
        public string CombinedName
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(Model.Email))
                    return $"{Model.Name} <{Model.Email}>";
                return Model.Name;
            }

            set
            {
                var regex = new Regex(@"'?(\w+(?:\s+\w+)*)'?\s+<?(\S+@[\w.-]+\.[a-zA-Z]{2,4}\b)");
                var match = regex.Match(value);

                if (match.Success && match.Groups.Count == 3)
                {
                    Model.Name = match.Groups[1].Value;
                    Model.Email = match.Groups[2].Value;
                }
                else
                {
                    Model.Name = value.Trim();
                    Model.Email = string.Empty;
                }

                OnPropertyChanged();
                OnPropertyChanged("NameControlVisibility");
                OnPropertyChanged("EmailControlVisibility");
            }
        }

        public Visibility NameControlVisibility => !string.IsNullOrWhiteSpace(Model.Name)
            ? Visibility.Visible
            : Visibility.Collapsed;

        public Visibility EmailControlVisibility => !string.IsNullOrWhiteSpace(Model.Email)
            ? Visibility.Visible
            : Visibility.Collapsed;
    }
}
