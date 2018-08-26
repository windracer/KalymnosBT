using System;
using System.Text.RegularExpressions;
using KalymnosBT.MVVMBase;

namespace KalymnosBT.Models
{
    public class Vote : ObservableObject
    {
        private string _name = string.Empty;
        private string _email = string.Empty;
        private DateTime _date = DateTime.UtcNow;

        public string Email
        {
            get => _email;
            set => SetProperty(ref _email, value);
        }

        public string Name 
        {
            get => _name;
            set
            {
                var regex = new Regex(@"'?(\w+(?:\s+\w+)*)'?\s+<?(\S+@[\w.-]+\.[a-zA-Z]{2,4}\b)");
                var match = regex.Match(value);

                if (match.Success && match.Groups.Count == 3)
                {
                    string name = match.Groups[1].Value;
                    string email = match.Groups[2].Value;
                    SetProperty(ref _name, name);
                    SetProperty(ref _email, email, "Email");
                }
                else
                {
                    SetProperty(ref _name, value);
                }
            }
        }

        public DateTime When
        {
            get => _date;
            set => SetProperty(ref _date, value);
        }

    }
}
