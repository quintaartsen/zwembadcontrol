using Tibber.Sdk;

namespace ZwembadControl.Connectors
{
    public interface ITibberConnector
    {
        Task<PriceInfo> GetPriceLevel();
    }
}


