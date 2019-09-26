using SistemaFichaje;
using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Media;
using System.Linq;

namespace SistemaFichajeWPF
{
    /// <summary>
    /// Lógica de interacción para ListadoPersonal.xaml
    /// </summary>
    public partial class ListadoPersonal : Page
    {
        private ObservableCollection<Personal> _ListaPersonal;
        private readonly DispatcherTimer timerLabel;

        public ObservableCollection<Personal> ListaPersonal { get { return this._ListaPersonal; } set { this._ListaPersonal = value; } }

        // Constructor
        #region ListadoPersonal
        public ListadoPersonal()
        {
            InitializeComponent();

            LectorTarjetas.LimpiaPantallaLCD();
            _ListaPersonal = AccessHelper.LeerDatosPersonal();

            #region Inicialización Timer
            timerLabel = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timerLabel.Tick += TimerLabel_Tick;
            #endregion

            lvPersonal.ItemsSource = ListaPersonal;
        }
        #endregion
        
        // Event Handler para el timer
        #region TimerLabel_Tick
        private void TimerLabel_Tick(object sender, EventArgs e)
        {
            tbInfo.Visibility = Visibility.Collapsed;
            timerLabel.Stop();
        }
        #endregion

        // Elimina el personal seleccionado
        #region BtEliminar_Click
        private void BtEliminar_Click(object sender, EventArgs e)
        {
            Personal selectedPersonal = (Personal)lvPersonal.SelectedItem;

            if (selectedPersonal != null)
            {
                if (AccessHelper.EliminaPersonal(selectedPersonal.Id))
                {
                    // Borra el usuario de la lista de personal
                    _ListaPersonal.Remove(
                                _ListaPersonal.Where(x => x.Id == selectedPersonal.Id)
                                .First()
                                );

                    // Activa el label info y luego el timer
                    tbInfo.Visibility = Visibility.Visible;
                    tbInfo.Foreground = new SolidColorBrush(Colors.DarkGreen);
                    tbInfo.Text = "Se ha eliminado el usuario correctamente.";
                    timerLabel.Start();
                }
                else
                {
                    // Activa el label info y luego el timer
                    tbInfo.Visibility = Visibility.Visible;
                    tbInfo.Foreground = new SolidColorBrush(Colors.DarkRed);
                    tbInfo.Text = "No se ha podido eliminar el usuario.";
                    timerLabel.Start();
                }
            }
        }
        #endregion

        // Abre una ventana para registrar a un nuevo usuario
        #region BtRegistrarPersonal_Click
        private void BtRegistrarPersonal_Click(object sender, RoutedEventArgs e)
        {
            RegistroPersonalDialog registroPersonal = new RegistroPersonalDialog();
            if (registroPersonal.ShowDialog() == true)
            {
                Personal personal = registroPersonal.Resultado;

                int resultCode = AccessHelper.InsertaNuevoPersonal(personal.Nombre, personal.HorasSemanales, personal.HorasAnuales, personal.Tarjeta);

                if (resultCode >= 0)
                {
                    personal.Id = resultCode;
                    _ListaPersonal.Add(personal);

                    tbInfo.Visibility = Visibility.Visible;
                    tbInfo.Foreground = new SolidColorBrush(Colors.DarkGreen);
                    tbInfo.Text = "Se ha completado el registro con éxito.";
                    timerLabel.Start();
                }
                else if (resultCode == -1)
                {
                    tbInfo.Visibility = Visibility.Visible;
                    tbInfo.Foreground = new SolidColorBrush(Colors.DarkRed);
                    tbInfo.Text = "La tarjeta leída ya está vinculada con un usuario. Lea otra tarjeta o seleccione Eliminar para borrar la actual.";
                    timerLabel.Start();
                }
                else if (resultCode == -2)
                {
                    tbInfo.Visibility = Visibility.Visible;
                    tbInfo.Foreground = new SolidColorBrush(Colors.DarkRed);
                    tbInfo.Text = "Ha habido un error con la base de datos y no se ha podido insertar la información.";
                    timerLabel.Start();
                }
                else if (resultCode == -3)
                {
                    tbInfo.Visibility = Visibility.Visible;
                    tbInfo.Foreground = new SolidColorBrush(Colors.DarkRed);
                    tbInfo.Text = "No se ha podido completar el registro.";
                    timerLabel.Start();
                }
            }
        }
        #endregion
    }
}
