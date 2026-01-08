using System.Threading;

namespace SistemaFichaje.CardReader.RfidDevices
{
    public interface IReaderDevice
    {
        long ReadCard(CancellationToken cancellationToken);
    }
}