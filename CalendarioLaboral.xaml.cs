using SistemaFichaje;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Linq;
using System.Windows.Threading;

namespace SistemaFichajeWPF
{
    /// <summary>
    /// Lógica de interacción para CalendarioLaboral.xaml
    /// </summary>
    public partial class CalendarioLaboral : Page
    {
        private ObservableCollection<FechaFestivo> _ListaFestivos;
        public ObservableCollection<FechaFestivo> ListaFestivos { get { return _ListaFestivos; } }
        private DispatcherTimer timerLabel;

        // Constructor
        #region CalendarioLaboral
        public CalendarioLaboral()
        {
            InitializeComponent();
            LectorTarjetas.LimpiaPantallaLCD();

            Cursor = Cursors.Wait;
            _ListaFestivos = AccessHelper.LeerDatosCalendario();
            Cursor = Cursors.Arrow;

            #region Inicialización Timer
            // DispatcherTimer
            timerLabel = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            timerLabel.Tick += TimerLabel_Tick;
            #endregion

            dpFestivos.DisplayDateStart = new DateTime(DateTime.Now.Year, 1, 1);
            dpFestivos.DisplayDateEnd = new DateTime(DateTime.Now.Year, 12, 31);

            // Vincula el listview con la lista de días de la BD
            lvFestivos.ItemsSource = _ListaFestivos;
        }
        #endregion

        #region TimerLabel_Tick
        private void TimerLabel_Tick(object sender, EventArgs e)
        {
            tbInfo.Visibility = Visibility.Collapsed;
            timerLabel.Stop();
        }
        #endregion

        // Añade a la BD el día seleccionado en el calendario
        #region BtAñadir_Click
        private void BtAñadir_Click(object sender, RoutedEventArgs e)
        {
            if (dpFestivos.SelectedDate != null)
            {
                DateTime date = (DateTime)dpFestivos.SelectedDate;
                if (AccessHelper.InsertaNuevoDiaFestivo(date.Date))
                {
                    _ListaFestivos.Add(new FechaFestivo(date.Date));
                }               
            }
        }
        #endregion

        // Añade todos los sábados y domingos del año
        #region BtAñadirSabDom_Click
        private async void BtAñadirSabDom_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            await Task.Factory.StartNew(() => AñadeSabadosDomingos());
            Cursor = Cursors.Arrow;
        }
        #endregion

        // Elimina de la BD el día seleccionado en el ListView
        #region BtEliminar_Click
        private void BtEliminar_Click(object sender, RoutedEventArgs e)
        {
            if (lvFestivos.SelectedItem != null) //(FechaFestivo)
            {
                DateTime date = ((FechaFestivo)lvFestivos.SelectedItem).Fecha;
                if (AccessHelper.EliminaDiaFestivo(date.Date))
                {
                    _ListaFestivos.Remove((FechaFestivo)lvFestivos.SelectedItem);
                }                
            }
        }
        #endregion

        // Vacia todos los días del ListView y se elimina de la BD
        #region BtVaciar_Click
        private async void BtVaciar_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;

            tbInfo.Text = "Se están eliminando los días registrados, esta operación puede tardar unos segundos.";
            tbInfo.Visibility = Visibility.Visible;

            await Task.Factory.StartNew(() =>
            {
                bool couldErase = false;
                foreach (FechaFestivo fechaFestivo in lvFestivos.Items)
                {
                    couldErase = AccessHelper.EliminaDiaFestivo(fechaFestivo.Fecha.Date);
                    if (!couldErase) break;
                }

                if (couldErase)
                {
                    Dispatcher.Invoke(() => { _ListaFestivos.Clear(); });
                }
            });

            Cursor = Cursors.Arrow;
            tbInfo.Text = "";
            tbInfo.Visibility = Visibility.Collapsed;
        }
        #endregion

        // Calcula todos los días laborables del año, según los festivos que hay
        #region BtCalculoLaborables_Click
        private async void BtCalculoLaborables_Click(object sender, RoutedEventArgs e)
        {
            Cursor = Cursors.Wait;
            tbInfo.Text = "Se están calculando los días laborables...";
            tbInfo.Visibility = Visibility.Visible;

            Task<int> calculoLaborables = Task.Run(() => CalcularDiasLaborables());
            int diasLaborables = await calculoLaborables;

            Cursor = Cursors.Arrow;
            tbInfo.Text = "El número de días laborables para este año és " + diasLaborables + ".";
            timerLabel.Start();
        }
        #endregion

        #region AñadeSabadosDomingos
        private void AñadeSabadosDomingos()
        {
            DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime endDate = new DateTime(DateTime.Now.Year, 12, 31);

            List<DateTime> allSatSuns = new List<DateTime>();
            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
                {
                    if (AccessHelper.InsertaNuevoDiaFestivo(date.Date))
                    {
                        Dispatcher.Invoke(() =>
                        {
                            _ListaFestivos.Add(new FechaFestivo(date.Date));
                        });
                    }
                }
            }
        }
        #endregion

        #region CalcularDiasLaborables
        private int CalcularDiasLaborables()
        {
            int contadorDias = 0;
            DateTime startDate = new DateTime(DateTime.Now.Year, 1, 1);
            DateTime endDate = new DateTime(DateTime.Now.Year, 12, 31);

            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                if (! _ListaFestivos.Any(ff => ff.Fecha.Date == date.Date))
                {
                    contadorDias++;
                }
            }

            return contadorDias;
        }
        #endregion
    }
}
