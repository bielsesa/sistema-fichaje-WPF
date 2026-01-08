using SistemaFichaje;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace SistemaFichajeWPF
{
    /// <summary>
    /// Lógica de interacción para Fichaje.xaml
    /// </summary>
    public partial class Fichaje : Page
    {
        // Constructor
        #region Fichaje
        public Fichaje()
        {
            InitializeComponent();
            LectorTarjetas.ClearScreen();
        }
        #endregion

        // Botón registro entrada
        #region BtEntrada_Click
        private void BtEntrada_Click(object sender, EventArgs e)
        {
            LecturaTarjetaRegistroHistorial(TipoRegistro.Entrada);
        }
        #endregion

        // Botón registro salida 
        #region BtSalida_Click
        private void BtSalida_Click(object sender, EventArgs e)
        {
            LecturaTarjetaRegistroHistorial(TipoRegistro.Salida);
        }
        #endregion

        // Botón registro descanso (salida)
        #region BtAlmuerzo_Click
        private void BtDescansoSalida_Click(object sender, EventArgs e)
        {
            LecturaTarjetaRegistroHistorial(TipoRegistro.DescansoSalida);
        }
        #endregion

        // Botón registro descanso (entrada)
        #region BtDescansoEntrada_Click
        private void BtDescansoEntrada_Click(object sender, EventArgs e)
        {
            LecturaTarjetaRegistroHistorial(TipoRegistro.DescansoEntrada);
        }
        #endregion
        
        // Botón registro comida (salida)
        #region BtComidaSalida_Click
        private void BtComidaSalida_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LecturaTarjetaRegistroHistorial(TipoRegistro.ComidaSalida);
        }
        #endregion

        //Botón registro comida (entrada)
        #region BtComidaEntrada_Click
        private void BtComidaEntrada_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            LecturaTarjetaRegistroHistorial(TipoRegistro.ComidaEntrada);
        }
        #endregion

        // Lee la tarjeta e introduce el tipo de registro correspondiente en la BD, 
        // con la hora y el ID del personal.
        #region LecturaTarjetaRegistroHistorial
        private async void LecturaTarjetaRegistroHistorial(TipoRegistro tipoRegistro)
        {
            LectorTarjetas.ShowMessageOnScreen("Por favor, pase la tarjeta");

            CancellationTokenSource source = new CancellationTokenSource();
            source.CancelAfter(TimeSpan.FromSeconds(10)); // Timeout de 10 segundos
            Task<long> leeTarjeta = Task.Run(() => LectorTarjetas.ReadCard(source.Token), source.Token);

            btEntrada.IsEnabled = false;
            btSalida.IsEnabled = false;
            btDescansoEntrada.IsEnabled = false;
            btDescansoSalida.IsEnabled = false;
            btComidaEntrada.IsEnabled = false;
            btComidaSalida.IsEnabled = false;

            long numTarjeta = await leeTarjeta;
            source.Dispose();

            // Inserta los datos del registro en la BD
            if (numTarjeta != -1)
            {
                SQLiteDatabase.InsertaNuevoRegistroHistorial(numTarjeta, tipoRegistro);
#if DEBUG
                GlobalData.PrintDebug("SistemaFichaje.cs", "[" + DateTime.Now + "] Se ha leído la tarjeta con número:" + numTarjeta + ".\n");
#endif

                string nombre = SQLiteDatabase.NombreDePersonalConIDTarjeta(numTarjeta), msg = "";

                if (nombre == "null")
                {
                    msg = String.Format("[{0:hh:mm:ss}] No se reconoce ID.", DateTime.Now);
                }
                else
                {
                    if (tipoRegistro == TipoRegistro.Entrada)
                    {
                        msg = String.Format("[{0:hh:mm:ss}] Entrada [{1}]", DateTime.Now, nombre);
                    }
                    else if (tipoRegistro == TipoRegistro.Salida)
                    {
                        msg = String.Format("[{0:hh:mm:ss}] Salida [{1}]", DateTime.Now, nombre);
                    }
                    else if (tipoRegistro == TipoRegistro.DescansoSalida)
                    {
                        msg = String.Format("[{0:hh:mm:ss}] Salida Descanso [{1}]", DateTime.Now, nombre);
                    }
                    else if (tipoRegistro == TipoRegistro.DescansoEntrada)
                    {
                        msg = String.Format("[{0:hh:mm:ss}] Entrada Descanso [{1}]", DateTime.Now, nombre);
                    }
                }
#if DEBUG
                GlobalData.PrintDebug("SistemaFichaje.cs", msg + "\n");
#endif
                LectorTarjetas.ClearScreen();
                LectorTarjetas.ShowMessageOnScreen(msg);
                LectorTarjetas.TimerLimpiaPantallaLCD();
            }
            else
            {
#if DEBUG
                GlobalData.PrintDebug("SistemaFichaje.cs", "[" + DateTime.Now + "] No se ha recibido ninguna tarjeta o el formato no es válido.");
#endif
                LectorTarjetas.ClearScreen();
                LectorTarjetas.ShowMessageOnScreen("No se reconoce ID");
                LectorTarjetas.TimerLimpiaPantallaLCD();
            }

            btEntrada.IsEnabled = true;
            btSalida.IsEnabled = true;
            btDescansoEntrada.IsEnabled = true;
            btDescansoSalida.IsEnabled = true;
            btComidaEntrada.IsEnabled = true;
            btComidaSalida.IsEnabled = true;
        }
        #endregion

    }
}
