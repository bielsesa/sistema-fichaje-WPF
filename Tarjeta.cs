using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SistemaFichaje
{
    public class Tarjeta : INotifyPropertyChanged
    {
        private string id;
        private Personal personal;
        public event PropertyChangedEventHandler PropertyChanged;

        #region NotifyPropertyChanged
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Id
        public string Id
        {
            get
            {
                return id;
            }

            set
            {
                if (id != value)
                {
                    id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Personal
        public Personal Personal
        {
            get
            {
                return personal;
            }

            set
            {
                if (personal != value)
                {
                    personal = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Tarjeta(string id)
        public Tarjeta(string id)
        {
            Id = id;
        }
        #endregion

        #region Tarjeta(string id, Personal personal)
        public Tarjeta(string id, Personal personal)
        {
            Id = id;
            Personal = personal;
        }
        #endregion

        #region GetPersonalNombre
        public string GetPersonalNombre()
        {
            return personal.Nombre;
        }
        #endregion
    }
}
