using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SistemaFichajeWPF
{
    public class FechaFestivo : INotifyPropertyChanged
    {
        private DateTime fecha;

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        // parameter causes the property name of the caller to be substituted as an argument.  
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public FechaFestivo(DateTime fecha)
        {
            Fecha = fecha;
        }

        public DateTime Fecha
        {
            get
            {
                return fecha;
            }

            set
            {
                if (value != this.fecha)
                {
                    this.fecha = value;
                    NotifyPropertyChanged();
                }
            }
        }
    }
}
