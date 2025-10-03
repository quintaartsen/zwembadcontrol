namespace ZwembadControl.Connectors
{
    public interface IHyconConnector
    {
        Task<HyconData> GetDataAsync();
    }
}


