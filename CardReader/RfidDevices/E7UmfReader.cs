using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace SistemaFichaje.CardReader.RfidDevices
{
    public class E7UmfReader : IReaderDevice, IReaderDeviceScreen
    {
        // [DllImport("E7umf.dll", EntryPoint = "fw_init")]
        // public static extern int fw_init(short port, int baud);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_config_card")]
        // public static extern int fw_config_card(int icdev, byte flags);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_exit")]
        // public static extern int fw_exit(int icdev);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_request")]
        // public static extern int fw_request(int icdev, byte _Mode, uint[] TagType);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_anticoll")]
        // public static extern int fw_anticoll(int icdev, byte _Bcnt, ulong[] _Snr);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_select")]
        // public static extern int fw_select(int icdev, uint _Snr, byte[] _Size);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_card")]
        // public static extern int fw_card(int icdev, byte _Mode, ulong[] _Snr);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_load_key")]
        // public static extern int fw_load_key(int icdev, byte _Mode, byte _SecNr, byte[] _NKey);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_authentication")]
        // public static extern int fw_authentication(int icdev, byte _Mode, byte _SecNr);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_read")]
        // public static extern int fw_read(int icdev, byte _Adr, byte[] _Data);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_read_hex")]
        // public static extern short fw_read_hex(int icdev, byte _Adr, StringBuilder _Data);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_write")]
        // public static extern int fw_write(int icdev, byte _Adr, byte[] _Data);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_write_hex")]
        // public static extern short fw_write_hex(int icdev, byte _Adr, string _Data);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_halt")]
        // public static extern int fw_halt(int icdev);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_changeb3")]
        // public static extern int fw_changeb3(int icdev, byte _SecNr, byte[] _KeyA, byte[] _CtrlW, byte _Bk, byte[] _KeyB);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_initval")]
        // public static extern int fw_initval(int icdev, byte _Adr, uint _Value);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_increment")]
        // public static extern int fw_increment(int icdev, byte _Adr, uint _Value);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_readval")]
        // public static extern int fw_readval(int icdev, byte _Adr, uint[] _Value);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_decrement")]
        // public static extern int fw_decrement(int icdev, byte _Adr, uint _Value);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_restore")]
        // public static extern int fw_restore(int icdev, byte _Adr);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_transfer")]
        // public static extern int fw_transfer(int icdev, byte _Adr);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_beep")]
        // public static extern int fw_beep(int icdev, uint _Msec);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_getver")]
        // public static extern int fw_getver(int icdev, byte[] buff);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_reset")]
        // public static extern short fw_reset(int icdev, ushort _Msec);
        //
        // [DllImport("E7umf.dll", EntryPoint = "hex_a")]
        // public static extern void hex_a(ref byte hex, byte[] a, short len);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_lcd_dispclear")]
        // public static extern int fw_lcd_dispclear(int icdev);
        //
        // [DllImport("E7umf.dll", EntryPoint = "fw_lcd_dispstr")]
        // private static extern int fw_lcd_dispstr(int icdev, string pText);

        public long ReadCard(CancellationToken cancellationToken)
        {
            // var cardNumber = new ulong[3];
            // var cardReaderDevice = ConnectCardReader();
            //
            // while (true)
            // {
            //     if (cancellationToken.IsCancellationRequested)
            //     {
            //         GlobalData.PrintDebug("LECTOR TARJETAS", "CancellationToken: " + cancellationToken.IsCancellationRequested);
            //         DisconnectCardReader(cardReaderDevice);
            //         break;
            //     }
            //
            //     // var state = fw_card(cardReaderDevice, 0x31, cardNumber);
            //     if (state != 0)
            //     {
            //         continue;
            //     }
            //     
            //     GlobalData.PrintDebug("TASK BUCLE LECTURA", "Se ha recibido una lectura. Num tarjeta: " + cardNumber[0]);
            //
            //     DisconnectCardReader(cardReaderDevice);
            //     return Convert.ToInt64(cardNumber[0]);
            // }

            return -1;
        }

        public void ShowMessage(string message)
        {
            GlobalData.PrintDebug("LECTOR TARJETAS", "Muestra mensaje");
            var cardReaderDevice = ConnectCardReader();
            // fw_lcd_dispstr(cardReaderDevice, message);
            DisconnectCardReader(cardReaderDevice);
        }

        public void ClearScreen()
        {
            GlobalData.PrintDebug("LECTOR TARJETAS", "Limpia pantalla");
            var cardReaderDevice = ConnectCardReader();
            // fw_lcd_dispclear(cardReaderDevice);
            DisconnectCardReader(cardReaderDevice);
        }
        
        private static int ConnectCardReader()
        {
            // return fw_init(100, 0);
            return 0;
        }

        private static void DisconnectCardReader(int cardReaderDevice)
        {
            Thread.Sleep(50);
            // fw_exit(cardReaderDevice);
        }
    }
}