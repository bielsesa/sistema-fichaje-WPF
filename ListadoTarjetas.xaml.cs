using SistemaFichaje;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace SistemaFichajeWPF
{
    /// <summary>
    /// Lógica de interacción para ListadoTarjetas.xaml
    /// </summary>
    public partial class ListadoTarjetas : Page
    {
        private ObservableCollection<Tarjeta> _ListaTarjetas;
        private ObservableCollection<Personal> _ListaPersonal;
        private DispatcherTimer timerLabel;

        public ObservableCollection<Tarjeta> ListaTarjetas
        {
            get
            {
                return _ListaTarjetas;
            }

            set
            {
                _ListaTarjetas = value;
            }
        }
        public ObservableCollection<Personal> ListaPersonal
        {
            get
            {
                return _ListaPersonal;
            }

            set
            {
                _ListaPersonal = value;
            }
        }

        public ListadoTarjetas()
        {
            InitializeComponent();

            LectorTarjetas.LimpiaPantallaLCD();

            #region Inicialización Timer
            // DispatcherTimer
            timerLabel = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timerLabel.Tick += TimerLabel_Tick;
            #endregion

            _ListaTarjetas = AccessHelper.LeerDatosTarjetas();
            _ListaPersonal = AccessHelper.LeerDatosPersonal(); 

            lvTarjetas.ItemsSource = ListaTarjetas;
            cbListaPersonal.ItemsSource = ListaPersonal;
        }

        #region TimerLabel_Tick
        private void TimerLabel_Tick(object sender, EventArgs e)
        {
            tbInfo.Visibility = Visibility.Collapsed;
            timerLabel.Stop();
        }
        #endregion

        #region BtDesvincular_Click
        private void BtDesvincular_Click(object sender, EventArgs e)
        {
            Tarjeta selectedTarjeta = (Tarjeta)lvTarjetas.SelectedItem;

            if (selectedTarjeta != null)
            {
                // Borra la tarjeta de la BD
                if (AccessHelper.DesvinculaTarjeta(selectedTarjeta.Id))
                {

                    // Borra la tarjeta de la lista temporal
                    _ListaTarjetas.Remove(
                                _ListaTarjetas.Where(x => x.Id == selectedTarjeta.Id)
                                .First()
                                );

                    // Activa el label info y luego el DispatcherTimer
                    tbInfo.Visibility = Visibility.Visible;
                    tbInfo.Foreground = new SolidColorBrush(Colors.DarkGreen);
                    tbInfo.Text = "Se ha desvinculado la tarjeta correctamente.";
                    timerLabel.Start();
                }
                else
                {
                    // Activa el label info y luego el DispatcherTimer
                    tbInfo.Visibility = Visibility.Visible;
                    tbInfo.Foreground = new SolidColorBrush(Colors.DarkRed);
                    tbInfo.Text = "No se ha podido desvincular la tarjeta.";
                    timerLabel.Start();
                }
            }
        }
        #endregion

        // Lee una tarjeta y muestra el número
        #region BtLeerTarjeta_Click
        private async void BtLeerTarjeta_Click(object sender, EventArgs e)
        {
            CancellationTokenSource source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromSeconds(10)); // Timeout de 10 segundos
            Task<long> leeTarjeta = Task.Run(() => LectorTarjetas.LecturaTarjeta(source.Token), source.Token);

            btLeerTarjeta.IsEnabled = false;

            long numTarjeta = await leeTarjeta;

            source.Dispose();

            // Inserta los datos del registro en la BD
            if (numTarjeta != -1)
            {
                tbNumTarjeta.Text = numTarjeta.ToString();
            }
            else
            {
                tbNumTarjeta.Text = "No se ha podido leer la tarjeta.";
            }

            btLeerTarjeta.IsEnabled = true;
        }
        #endregion

        // Guarda la información de la tarjeta y el usuario
        #region BtVincular_Click
        private void BtVincular_Click(object sender, EventArgs e)
        {
            long numTarjeta = String.IsNullOrEmpty(tbNumTarjeta.Text) || String.IsNullOrWhiteSpace(tbNumTarjeta.Text) ? 0 : long.Parse(tbNumTarjeta.Text);
            int idPersonal = cbListaPersonal.SelectedItem == null ? 0 : (cbListaPersonal.SelectedItem as Personal).Id;

            if (idPersonal == 0 || numTarjeta == 0)
            {
                MessageBox.Show("No se han rellenado todos los campos necesarios.",
                    "", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                if (AccessHelper.RegistroTarjeta(numTarjeta, idPersonal))
                {
                    Tarjeta tarjeta = new Tarjeta(numTarjeta.ToString(), 
                        new Personal(idPersonal, AccessHelper.NombreDePersonalConIDPersonal(idPersonal)));

                    ListaTarjetas.Add(tarjeta);

                    tbInfo.Foreground = new SolidColorBrush(Colors.DarkGreen);
                    tbInfo.Text = "Se ha registrado la tarjeta correctamente.";
                    tbInfo.Visibility = Visibility.Visible;
                    timerLabel.Start();
                }
                else
                {
                    tbInfo.Foreground = new SolidColorBrush(Colors.DarkRed);
                    tbInfo.Text = "No se ha podido registrar la tarjeta.";
                    tbInfo.Visibility = Visibility.Visible;
                    timerLabel.Start();
                }
            }
        }
        #endregion
    }
}
