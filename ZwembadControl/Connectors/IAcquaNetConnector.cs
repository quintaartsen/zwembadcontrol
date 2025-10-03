namespace ZwembadControl.Connectors
{
    public interface IAcquaNetConnector
    {
        Task StartSpoelenAsync();
        Task<string> GetDataAsync();
    }
}


