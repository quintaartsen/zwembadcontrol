using ZwembadControl.Connectors;
using ZwembadControl.Models;

namespace ZwembadControl.Controllers;

public class ZwembadServiceActies(IRelayConnector relayConnector, IAirWellConnector airWellConnector, ITibberConnector tibberConnector, IHyconConnector hyconConnector, IAcquaNetConnector acquaNetConnector)
{
    public async Task StartAirwellWarmtePompasync()
    {
        if (CurrentState.Instance.AirwellWarmtePomp != true)
        {
            await airWellConnector.SwitchHotWater(true);
            await airWellConnector.SwitchHeating(true);
            CurrentState.Instance.AirwellWarmtePomp = true;
        }
    }

    public async Task StopAirwellWarmtePompasync()
    {
        if (CurrentState.Instance.AirwellWarmtePomp != false)
        {
            await airWellConnector.SwitchHotWater(false);
            await airWellConnector.SwitchHeating(false);
            CurrentState.Instance.AirwellWarmtePomp = false;
        }
    }

    public async Task SetLowTempAirwellWarmtePompasync()
    {
        var value = 50;
        if (CurrentState.Instance.TargetBoilerWaterTemp != value || CurrentState.Instance.TargetBufferWaterTemp != value)
        {
            await airWellConnector.SetBoilerTemp(value);
            await airWellConnector.SetWaterTemp(value);
        }
    }

    public async Task SetNormalTempAirwellWarmtePompasync()
    {
        var value = 50;
        if (CurrentState.Instance.TargetBoilerWaterTemp != value || CurrentState.Instance.TargetBufferWaterTemp != value)
        {
            await airWellConnector.SetBoilerTemp(value);
            await airWellConnector.SetWaterTemp(value);
        }
    }

    public async Task SetHighTempAirwellWarmtePompasync()
    {
        var value = 55;
        if (CurrentState.Instance.TargetBoilerWaterTemp != value || CurrentState.Instance.TargetBufferWaterTemp != value)
        {
            await airWellConnector.SetBoilerTemp(50);
            await airWellConnector.SetWaterTemp(value);
        }
    }

    public async Task StartZwembadPompAsync()
    {
        relayConnector.CloseRelay(RelayConfig.PompZwembadWarmtePomp);
    }

    public async Task StopZwembadPompAsync()
    {
        relayConnector.OpenRelay(RelayConfig.PompZwembadWarmtePomp);
    }


    public async Task StartZwembadWarmtePompAsync()
    {
        CurrentState.Instance.ZwembadWarmtePomp = true;
        if(CurrentState.Instance.ZwembadPompMode == "auto")
        {
            relayConnector.CloseRelay(RelayConfig.PompZwembadWarmtePomp);
        }
        relayConnector.CloseRelay(RelayConfig.ZwembadWarmtePomp);
    }

    public async Task StopZwembadWarmtePompAsync()
    {
        CurrentState.Instance.ZwembadWarmtePomp = false;
        relayConnector.OpenRelay(RelayConfig.ZwembadWarmtePomp);

        if (CurrentState.Instance.ZwembadPompMode == "auto")
        {
            relayConnector.OpenRelay(RelayConfig.PompZwembadWarmtePomp);
        }
    }

    public async Task StartKlimaatSysteemasync()
    {
        CurrentState.Instance.KlimaatSysteem = true;
        relayConnector.CloseRelay(RelayConfig.KlimaatSysteem);
    }

    public async Task StopKlimaatSysteemasync()
    {
        CurrentState.Instance.KlimaatSysteem = false;
        relayConnector.OpenRelay(RelayConfig.KlimaatSysteem);
    }

    public async Task StartLegionellasync()
    {
        CurrentState.Instance.LegionellaBoiler = true;
        CurrentState.Instance.Spoelen = "aan";
        relayConnector.CloseRelay(RelayConfig.LegionellaBoiler);
    }

    public async Task StopLegionellasync()
    {
        CurrentState.Instance.LegionellaBoiler = false;
        CurrentState.Instance.Spoelen = "uit";
        relayConnector.OpenRelay(RelayConfig.LegionellaBoiler);
    }

    public async Task OpenBoilerKlepAsync()
    {
        if (CurrentState.Instance.BoilerKlepOpen != true)
        {
            CurrentState.Instance.BoilerKlepOpen = true;
            relayConnector.CloseRelay(RelayConfig.BoilerKlepOpen);
            await Task.Delay(10000);
            relayConnector.OpenRelay(RelayConfig.BoilerKlepOpen);
        }
    }

    public async Task CloseBoilerKlepAsync()
    {

        if (CurrentState.Instance.BoilerKlepOpen != false)
        {
            CurrentState.Instance.BoilerKlepOpen = false;

            relayConnector.CloseRelay(RelayConfig.BoilerKlepDicht);
            await Task.Delay(10000);// full is 30 seconde
            relayConnector.OpenRelay(RelayConfig.BoilerKlepDicht);
        }
    }

    public async Task OpenZwembadKlepAsync()
    {
        CurrentState.Instance.ZwembadKlepOpen = true;

        relayConnector.CloseRelay(RelayConfig.ZwembadKlepOpen);
        relayConnector.CloseRelay(RelayConfig.ZwembadKlepDicht);
    }

    public async Task CloseZwembadKlepAsync()
    {
        CurrentState.Instance.ZwembadKlepOpen = false;

        relayConnector.OpenRelay(RelayConfig.ZwembadKlepOpen);
        relayConnector.OpenRelay(RelayConfig.ZwembadKlepDicht);
    }
}
