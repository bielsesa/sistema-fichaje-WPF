using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace SistemaFichaje
{
    public static class LectorTarjetas
    {
        #region DLLImports
        [DllImport("E7umf.dll", EntryPoint = "fw_init")]
        public static extern Int32 fw_init(Int16 port, Int32 baud);
        [DllImport("E7umf.dll", EntryPoint = "fw_config_card")]
        public static extern Int32 fw_config_card(Int32 icdev, Byte flags);
        [DllImport("E7umf.dll", EntryPoint = "fw_exit")]
        public static extern Int32 fw_exit(Int32 icdev);
        [DllImport("E7umf.dll", EntryPoint = "fw_request")]
        public static extern Int32 fw_request(Int32 icdev, Byte _Mode, UInt32[] TagType);
        [DllImport("E7umf.dll", EntryPoint = "fw_anticoll")]
        public static extern Int32 fw_anticoll(Int32 icdev, Byte _Bcnt, ulong[] _Snr);
        [DllImport("E7umf.dll", EntryPoint = "fw_select")]
        public static extern Int32 fw_select(Int32 icdev, UInt32 _Snr, Byte[] _Size);
        [DllImport("E7umf.dll", EntryPoint = "fw_card")]
        public static extern Int32 fw_card(Int32 icdev, Byte _Mode, ulong[] _Snr);
        [DllImport("E7umf.dll", EntryPoint = "fw_load_key")]
        public static extern Int32 fw_load_key(Int32 icdev, Byte _Mode, Byte _SecNr, Byte[] _NKey);
        [DllImport("E7umf.dll", EntryPoint = "fw_authentication")]
        public static extern Int32 fw_authentication(Int32 icdev, Byte _Mode, Byte _SecNr);
        [DllImport("E7umf.dll", EntryPoint = "fw_read")]
        public static extern Int32 fw_read(Int32 icdev, Byte _Adr, Byte[] _Data);

        [DllImport("E7umf.dll", EntryPoint = "fw_read_hex")]
        public static extern Int16 fw_read_hex(Int32 icdev, Byte _Adr, StringBuilder _Data);

        [DllImport("E7umf.dll", EntryPoint = "fw_write")]
        public static extern Int32 fw_write(Int32 icdev, Byte _Adr, Byte[] _Data);

        [DllImport("E7umf.dll", EntryPoint = "fw_write_hex")]
        public static extern Int16 fw_write_hex(Int32 icdev, Byte _Adr, string _Data);

        [DllImport("E7umf.dll", EntryPoint = "fw_halt")]
        public static extern Int32 fw_halt(Int32 icdev);
        [DllImport("E7umf.dll", EntryPoint = "fw_changeb3")]
        public static extern Int32 fw_changeb3(Int32 icdev, Byte _SecNr, Byte[] _KeyA, Byte[] _CtrlW, Byte _Bk,
                 Byte[] _KeyB);
        [DllImport("E7umf.dll", EntryPoint = "fw_initval")]
        public static extern Int32 fw_initval(Int32 icdev, Byte _Adr, UInt32 _Value);
        [DllImport("E7umf.dll", EntryPoint = "fw_increment")]
        public static extern Int32 fw_increment(Int32 icdev, Byte _Adr, UInt32 _Value);
        [DllImport("E7umf.dll", EntryPoint = "fw_readval")]
        public static extern Int32 fw_readval(Int32 icdev, Byte _Adr, UInt32[] _Value);
        [DllImport("E7umf.dll", EntryPoint = "fw_decrement")]
        public static extern Int32 fw_decrement(Int32 icdev, Byte _Adr, UInt32 _Value);
        [DllImport("E7umf.dll", EntryPoint = "fw_restore")]
        public static extern Int32 fw_restore(Int32 icdev, Byte _Adr);
        [DllImport("E7umf.dll", EntryPoint = "fw_transfer")]
        public static extern Int32 fw_transfer(Int32 icdev, Byte _Adr);
        [DllImport("E7umf.dll", EntryPoint = "fw_beep")]
        public static extern Int32 fw_beep(Int32 icdev, UInt32 _Msec);
        [DllImport("E7umf.dll", EntryPoint = "fw_getver")]
        public static extern Int32 fw_getver(Int32 icdev, byte[] buff);
        [DllImport("E7umf.dll", EntryPoint = "fw_reset")]
        public static extern Int16 fw_reset(Int32 icdev, UInt16 _Msec);
        [DllImport("E7umf.dll", EntryPoint = "hex_a")]
        public static extern void hex_a(ref Byte hex, Byte[] a, Int16 len);

        //lcd
        [DllImport("E7umf.dll", EntryPoint = "fw_lcd_dispclear")]
        public static extern Int32 fw_lcd_dispclear(Int32 icdev);
        [DllImport("E7umf.dll", EntryPoint = "fw_lcd_dispstr")]
        public static extern Int32 fw_lcd_dispstr(Int32 icdev, String pText);
        #endregion

        private static readonly DispatcherTimer timerLCD;
        private static int dispositivoRFID;

        // Constructor estático
        #region LectorTarjetas
        static LectorTarjetas ()
        {
            timerLCD = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(6)
            };
            timerLCD.Tick += TimerLCD_Tick;
        }
        #endregion

        #region Timer
        /**************** Timer ****************/
        private static void TimerLCD_Tick(object sender, EventArgs e)
        {
            LectorTarjetas.LimpiaPantallaLCD();
            timerLCD.Stop();
        }

        public static void TimerLimpiaPantallaLCD()
        {
            timerLCD.Start();
        }
        /**************** Timer ****************/
        #endregion
        
        // Abre el puerto USB del dispositivo.
        #region AbrirPuertoUSB
        public static bool AbrirPuertoUSB(out int dispositivoRFID)
        {
            dispositivoRFID = fw_init(100, 0);

            if (dispositivoRFID > 0)
            {
                return true;
            }
            else
            {
                dispositivoRFID = 0;
                return false;
            }
        }
        #endregion

        // Cierra el puerto USB del dispositivo.
        #region CerrarPuertoUSB
        public static void CerrarPuertoUSB(int dispositivoRFID)
        {
            Thread.Sleep(50);
            fw_exit(dispositivoRFID);
        }
        #endregion

        // Lee una tarjeta
        #region LecturaTarjeta
        public static long LecturaTarjeta(CancellationToken cancellationToken)
        {
            Byte[] revbuf = new Byte[64];
            ulong[] numeroTarjeta = new ulong[3];

            AbrirPuertoUSB(out dispositivoRFID);

            Int32 state;

            // Comprueba si se quiere cancelar la tarea antes de volver a entrar en el bucle
            while (true)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    GlobalData.PrintDebug("LECTOR TARJETAS", "CancellationToken: " + cancellationToken.IsCancellationRequested);
                    CerrarPuertoUSB(dispositivoRFID);
                    break;
                }

                state = fw_card(dispositivoRFID, 0x31, numeroTarjeta);
                if (state == 0)
                {
                    GlobalData.PrintDebug("TASK BUCLE LECTURA", "Se ha recibido una lectura. Num tarjeta: " + numeroTarjeta[0]);

                    CerrarPuertoUSB(dispositivoRFID);
                    return Convert.ToInt64(numeroTarjeta[0]);
                }
            }

            return -1;
        }
        #endregion

        // Muestra un mensaje en la pantalla LCD
        #region MuestraMensajeLCD
        public static void MuestraMensajeLCD(string msg)
        {
            GlobalData.PrintDebug("LECTOR TARJETAS", "Muestra mensaje");
            AbrirPuertoUSB(out dispositivoRFID);
            fw_lcd_dispstr(dispositivoRFID, msg);
            CerrarPuertoUSB(dispositivoRFID);
        }
        #endregion

        // Limpia la pantalla LCD de texto
        #region LimpiaPantallaLCD
        public static void LimpiaPantallaLCD()
        {
            GlobalData.PrintDebug("LECTOR TARJETAS", "Limpia pantalla");
            AbrirPuertoUSB(out dispositivoRFID);
            fw_lcd_dispclear(dispositivoRFID);
            CerrarPuertoUSB(dispositivoRFID);
        }
        #endregion
    }
}
