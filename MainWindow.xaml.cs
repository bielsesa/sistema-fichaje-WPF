using System;
using System.Windows;
using System.Windows.Threading;
using SistemaFichaje;

namespace SistemaFichajeWPF_01
{
    /// <summary>
    /// Lógica de interacción para MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DispatcherTimer timerTiempo = new DispatcherTimer();

        public MainWindow()
        {
            InitializeComponent();
            
            SQLiteDatabase.InicializarBaseDeDatos();

            tblFecha.Text = String.Format("{0:dddd} {0:dd} de {0:MMMM} de {0:yyyy}", DateTime.Now);

            #region Inicialización timer y TextBlock hora
            tblHora.Text = String.Format("{0:t}", DateTime.Now);
            timerTiempo.Tick += TimerTiempo_Tick;
            timerTiempo.Start();
            #endregion
        }

        private void TimerTiempo_Tick(object sender, EventArgs e)
        {
            tblHora.Text = String.Format("{0:t}", DateTime.Now);
        }

        private void BtFichaje_Click(object sender, RoutedEventArgs e)
        {
            btFichaje.IsEnabled = false;

            if (!btListadoPersonal.IsEnabled) btListadoPersonal.IsEnabled = true;
            if (!btListadoTarjetas.IsEnabled) btListadoTarjetas.IsEnabled = true;
            if (!btCalendario.IsEnabled) btCalendario.IsEnabled = true;

            framePrincipal.NavigationService.Navigate(
                new Uri("Fichaje.xaml", UriKind.Relative)
                );
        }

        private void BtListadoPersonal_Click(object sender, RoutedEventArgs e)
        {
            btListadoPersonal.IsEnabled = false;

            if (!btFichaje.IsEnabled) btFichaje.IsEnabled = true;
            if (!btListadoTarjetas.IsEnabled) btListadoTarjetas.IsEnabled = true;
            if (!btCalendario.IsEnabled) btCalendario.IsEnabled = true;

            framePrincipal.NavigationService.Navigate(
                new Uri("ListadoPersonal.xaml", UriKind.Relative)
                );
        }

        private void BtListadoTarjetas_Click(object sender, RoutedEventArgs e)
        {
            btListadoTarjetas.IsEnabled = false;

            if (!btFichaje.IsEnabled) btFichaje.IsEnabled = true;
            if (!btListadoPersonal.IsEnabled) btListadoPersonal.IsEnabled = true;
            if (!btCalendario.IsEnabled) btCalendario.IsEnabled = true;

            framePrincipal.NavigationService.Navigate(
                new Uri("ListadoTarjetas.xaml", UriKind.Relative)
                );
        }

        private void BtCalendario_Click(object sender, RoutedEventArgs e)
        {
            btCalendario.IsEnabled = false;

            if (!btFichaje.IsEnabled) btFichaje.IsEnabled = true;
            if (!btListadoPersonal.IsEnabled) btListadoPersonal.IsEnabled = true;
            if (!btListadoTarjetas.IsEnabled) btListadoTarjetas.IsEnabled = true;

            framePrincipal.NavigationService.Navigate(
                new Uri("CalendarioLaboral.xaml", UriKind.Relative)
                );
        }

        private void BtSalir_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
