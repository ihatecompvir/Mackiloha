using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GemToolkit.ViewModel
{
    public abstract class BaseModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void Set<T>(ref T pnv, T value, string propName)
        {
            pnv = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
            AnnouceChange(value, propName);
        }

        protected void AnnouceChange<T>(T value, string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
