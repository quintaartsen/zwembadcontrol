using Tibber.Sdk;
using ZwembadControl.Connectors;
using ZwembadControl.flows;
using ZwembadControl.Models;
using ZwembadControl.Stores;

namespace ZwembadControl.Controllers;

public class ZwembadService
{
    private readonly IAirWellConnector airWellConnector;
    private readonly ITibberConnector tibberConnector;
    private readonly IHyconConnector hyconConnector;

    private readonly FileDatabase<DateModel> database;

    private readonly IList<Flow> flows;

    public ZwembadService(IAirWellConnector airWellConnector, ITibberConnector tibberConnector, IHyconConnector hyconConnector, IList<Flow> flows)
    {
        this.airWellConnector = airWellConnector;
        this.tibberConnector = tibberConnector;
        this.hyconConnector = hyconConnector;
        this.flows = flows;

        database = new FileDatabase<DateModel>("./state.data");
        var dataEntryPoint = database.GetAll();
        if (database.GetAll().Count != 0)
        {
            CurrentState.Reset(dataEntryPoint.Last());
        }
    }

    private void StoreState()
    {
        database.Save(CurrentState.Instance);
    }


    public List<DateModel> GetStateHistory()
    {
        return database.GetAll().Where(t => t.currentDateTime.Month == DateTime.Now.Month && t.currentDateTime.Year == DateTime.Now.Year).ToList();
    }

    public async Task RunJob()
    {
        var priceInfo = await tibberConnector.GetPriceLevel();
        var airWellData = await airWellConnector.GetDataAsync();
        var hyconData = await hyconConnector.GetDataAsync();

        PriceLevel priceLevel = CalculateCurrentPriceLevel(priceInfo.Today, priceInfo.Current).GetValueOrDefault(PriceLevel.Normal);

        CurrentState.Instance.TargetBoilerWaterTemp = airWellData.Airwell_MIDEA_CAC_Basic_0_hotWaterTemperature_0__0_7_0_last;
        CurrentState.Instance.CurrentBoilerWaterTemp = airWellData.Airwell_MIDEA_CAC_Basic_0_waterTankTemperature_0__0_21_0_last;
        CurrentState.Instance.TargetBufferWaterTemp = airWellData.Airwell_MIDEA_CAC_Basic_0_temperatureZone1_0__0_5_0_last;
        CurrentState.Instance.CurrentPriceLevel = priceLevel.ToString();
        CurrentState.Instance.TargetZwembadWaterTemp = hyconData.TargetTemperature;
        CurrentState.Instance.CurrentZwembadWaterTemp = hyconData.CurrentTemperature;
        CurrentState.Instance.TargetZwembadWaterPH = hyconData.TargetPh;
        CurrentState.Instance.CurrentZwembadWaterPH = hyconData.CurrentPh;
        CurrentState.Instance.TargetZwembadWaterChloor = hyconData.TargetChloor;
        CurrentState.Instance.CurrentZwembadWaterChloor = hyconData.CurrentChloor;
        CurrentState.Instance.TargetZwembadWaterFlow = hyconData.TargetFlow;
        CurrentState.Instance.CurrentZwembadWaterFlow = hyconData.CurrentFlow;
        CurrentState.Instance.currentDateTime = DateTime.UtcNow;

        foreach (var item in flows)
        {
            await item.ExecuteAsync(priceInfo, priceInfo.Current.Total ?? default, priceLevel, airWellData, hyconData);
        }

        StoreState();
    }


    internal PriceLevel? CalculateCurrentPriceLevel(ICollection<Price> prices, Price currentPrice)
    {
        if (prices == null)
            return currentPrice.Level;

        PriceLevel? currentPriceLevel = null;

        foreach (var price in prices)
        {
            if (price.StartsAt == currentPrice.StartsAt)
            {
                currentPriceLevel = price.Level;
                continue;
            }

            if (currentPriceLevel.HasValue && price.Level.HasValue)
            {
                if (currentPriceLevel.Value != price.Level.Value)
                {
                    var result = price.Level < currentPriceLevel;

                    return currentPrice.Level == PriceLevel.Normal && result ? PriceLevel.Expensive : currentPrice.Level;

                }
            }
        }

        return currentPrice.Level;
    }
}
