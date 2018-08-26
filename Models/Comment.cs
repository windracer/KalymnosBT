using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KalymnosBT.MVVMBase;

namespace KalymnosBT.Models
{
    public class Comment : ObservableObject
    {
        private string _text = string.Empty;
        private DateTime _date = DateTime.UtcNow;

        public string Text { get => _text; set => SetProperty(ref _text, value); }
        public DateTime Date { get => _date; set => SetProperty(ref _date, value); }
    }
}
