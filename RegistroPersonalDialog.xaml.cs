using SistemaFichaje;
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace SistemaFichajeWPF
{
    /// <summary>
    /// Lógica de interacción para RegistroPersonalDialog.xaml
    /// </summary>
    public partial class RegistroPersonalDialog : Window
    {
        private readonly DispatcherTimer timerLabel;
        private Personal personal;

        public Personal Resultado { get { return this.personal; } }

        public RegistroPersonalDialog()
        {
            InitializeComponent();
            LectorTarjetas.LimpiaPantallaLCD();

            // Timer para la label de información 
            timerLabel = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timerLabel.Tick += TimerLabel_Tick;
        }


        #region TimerLabel_Tick
        private void TimerLabel_Tick(object sender, EventArgs e)
        {
            tbInfo.Visibility = Visibility.Collapsed;
            timerLabel.Stop();
        }
        #endregion

        #region BtRegistrar_Click
        private void BtRegistrar_Click(object sender, EventArgs e)
        {
            Regex numRegex = new Regex(@"^[0-9]+$");

            // Comprueba que las textboxes no están vacías 
            // o simplemente tienen espacios
            string nombre = String.IsNullOrEmpty(tbNombre.Text) || String.IsNullOrWhiteSpace(tbNombre.Text) ? null : tbNombre.Text;
            int horasSemanales = String.IsNullOrEmpty(tbHorasSemanales.Text) || String.IsNullOrWhiteSpace(tbHorasSemanales.Text) || !numRegex.IsMatch(tbHorasSemanales.Text) ? 0 : int.Parse(tbHorasSemanales.Text);
            int horasAnuales = String.IsNullOrEmpty(tbHorasAnuales.Text) || String.IsNullOrWhiteSpace(tbHorasAnuales.Text) || !numRegex.IsMatch(tbHorasAnuales.Text) ? 0 : int.Parse(tbHorasAnuales.Text);

            if (nombre == null || horasSemanales == 0 || horasAnuales == 0)
            {
                MessageBox.Show("No ha rellenado todos los campos requeridos o el formato no es válido.\nRellénelos e inténtelo de nuevo.",
                    "", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            else
            {
                personal = new Personal(nombre, horasSemanales, horasAnuales, tblNumTarjeta.Text);
                DialogResult = true;
                Close();
            }
        }
        #endregion

        #region BtLeerTarjeta_Click
        private async void BtLeerTarjeta_Click(object sender, EventArgs e)
        {
            LectorTarjetas.MuestraMensajeLCD("Por favor, pase la tarjeta");

            CancellationTokenSource source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromSeconds(10)); // Timeout de 10 segundos
            Task<long> leeTarjeta = Task.Run(() => LectorTarjetas.LecturaTarjeta(source.Token), source.Token);

            btLeerTarjeta.IsEnabled = false;

            long numTarjeta = await leeTarjeta;

            source.Dispose();

            if (AccessHelper.ExisteTarjeta(numTarjeta))
            {
                tblNumTarjeta.Text = "Esta tarjeta ya está vinculada.";
                LectorTarjetas.LimpiaPantallaLCD();
                LectorTarjetas.MuestraMensajeLCD("Esta tarjeta ya está vinculada.");
                LectorTarjetas.TimerLimpiaPantallaLCD();
            }
            else if (numTarjeta != -1)
            {
                tblNumTarjeta.Text = numTarjeta.ToString();
                LectorTarjetas.LimpiaPantallaLCD();
                LectorTarjetas.MuestraMensajeLCD("Num Tarjeta: " + numTarjeta);
                LectorTarjetas.TimerLimpiaPantallaLCD();
            }
            else
            {
                tblNumTarjeta.Text = "No se ha podido leer la tarjeta.";
                LectorTarjetas.LimpiaPantallaLCD();
                LectorTarjetas.MuestraMensajeLCD("No se ha podido leer la tarjeta.");
                LectorTarjetas.TimerLimpiaPantallaLCD();
            }

            btLeerTarjeta.IsEnabled = true;
        }
        #endregion

        #region BtEliminarTarjeta_Click
        private void BtEliminarTarjeta_Click(object sender, RoutedEventArgs e)
        {
            tblNumTarjeta.Text = "";
        }
        #endregion
    }
}
