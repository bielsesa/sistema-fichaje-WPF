using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SistemaFichaje
{
    public class Personal : INotifyPropertyChanged
    {
        private int id;
        private string nombre;
        private int horasSemanales;
        private int horasAnuales;
        private string tarjeta;

        public event PropertyChangedEventHandler PropertyChanged;

        #region NotifyPropertyChanged
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region Personal(int id)
        public Personal(int id)
        {
            Id = id;
        }
        #endregion

        #region Personal(int id, string nombre)
        public Personal(int id, string nombre)
        {
            Id = id;
            Nombre = nombre;
        }
        #endregion

        #region Personal(string nombre, int horasSemanales, int horasAnuales) 
        public Personal(string nombre, int horasSemanales, int horasAnuales)
        {
            Nombre = nombre;
            HorasSemanales = horasSemanales;
            HorasAnuales = horasAnuales;
        }
        #endregion

        #region Personal(string nombre, int horasSemanales, int horasAnuales, string tarjeta) 
        public Personal(string nombre, int horasSemanales, int horasAnuales, string tarjeta)
        {
            Nombre = nombre;
            HorasSemanales = horasSemanales;
            HorasAnuales = horasAnuales;
            Tarjeta = tarjeta;
        }
        #endregion

        #region Id
        public int Id
        {
            get
            {
                return this.id;
            }

            set
            {
                if (this.id != value)
                {
                    this.id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region Nombre
        public string Nombre
        {
            get
            {
                return this.nombre;
            }

            set
            {
                if (this.nombre != value)
                {
                    this.nombre = value;
                    NotifyPropertyChanged();
                }
            }
        }
        #endregion

        #region HorasSemanales
        public int HorasSemanales
        {
            get { return horasSemanales; }
            set { horasSemanales = value; }
        }
        #endregion

        #region HorasAnuales
        public int HorasAnuales
        {
            get { return horasSemanales; }
            set { horasSemanales = value; }
        }
        #endregion

        #region Tarjeta
        public string Tarjeta
        {
            get { return tarjeta; }
            set { tarjeta = value; }
        }
        #endregion

        #region ToString
        public override string ToString()
        {
            return Nombre;
        }
        #endregion
    }
}
