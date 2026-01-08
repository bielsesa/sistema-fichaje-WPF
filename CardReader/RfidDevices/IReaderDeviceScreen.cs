namespace SistemaFichaje.CardReader.RfidDevices
{
    public interface IReaderDeviceScreen
    {
        void ShowMessage(string message);
        
        void ClearScreen();
    }
}