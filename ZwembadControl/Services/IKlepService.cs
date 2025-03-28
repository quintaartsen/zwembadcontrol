namespace ZwembadControl.Services
{
    public interface IKlepService
    {
        void StartLegionellaBoiler();
        void StopLegionellaBoiler();

        void OpenBoiler();
        void CloseBoiler();

        void OpenZwembad();
        void CloseZwembad();

        void StartZwembadWarmtePomp();
        void StopZwembadWarmtePomp();
    }
}
