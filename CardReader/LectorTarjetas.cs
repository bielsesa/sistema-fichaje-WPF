using System;
using System.Threading;
using System.Windows.Threading;
using SistemaFichaje.CardReader.RfidDevices;

namespace SistemaFichaje
{
    public static class LectorTarjetas
    {
        private static readonly DispatcherTimer timerLCD;
        private static int cardReaderDevice;

        private static IReaderDevice _readerDevice;
        private static IReaderDeviceScreen _readerDeviceScreen;

        // Constructor estático
        #region LectorTarjetas
        static LectorTarjetas()
        {
            timerLCD = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(6)
            };
            timerLCD.Tick += TimerLCD_Tick;

            var reader = new E7UmfReader();
            
            _readerDevice = reader;
            _readerDeviceScreen = reader;
        }
        #endregion

        #region Timer
        private static void TimerLCD_Tick(object sender, EventArgs e)
        {
            ClearScreen();
            timerLCD.Stop();
        }

        public static void TimerLimpiaPantallaLCD()
        {
            timerLCD.Start();
        }
        #endregion

        public static long ReadCard(CancellationToken cancellationToken)
        {
            return _readerDevice.ReadCard(cancellationToken);
        }

        public static void ShowMessageOnScreen(string message)
        {
            _readerDeviceScreen.ShowMessage(message);
        }

        public static void ClearScreen()
        {
            _readerDeviceScreen.ClearScreen();
        }
    }
}
