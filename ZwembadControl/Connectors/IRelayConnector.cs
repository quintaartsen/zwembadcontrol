namespace ZwembadControl.Connectors
{
    public interface IRelayConnector
    {
        void OpenRelay(int pin);
        void CloseRelay(int pin);
    }
}


