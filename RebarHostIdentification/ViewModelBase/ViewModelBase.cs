using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler eventHandler = this.PropertyChanged;
            if (eventHandler == null) return;
            eventHandler(this, new PropertyChangedEventArgs(propertyName));

        }
    }
}

