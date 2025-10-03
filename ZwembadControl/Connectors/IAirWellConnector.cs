using ZwembadControl.Models;

namespace ZwembadControl.Connectors
{
    public interface IAirWellConnector
    {
        Task<AirWellData> GetDataAsync();
        Task SwitchHotWater(bool value);
        Task SwitchHeating(bool value);
        Task SetBoilerTemp(int value);
        Task SetWaterTemp(int value);
    }
}


