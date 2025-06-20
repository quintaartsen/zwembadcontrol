using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO.Pipelines;
using Tibber.Sdk;
using ZwembadControl.Connectors;
using ZwembadControl.Models;
using ZwembadControl.Stores;

namespace ZwembadControl.Controllers
{
    public class ZwembadService
    {
        private readonly RelayConnector relayConnector;
        private readonly AirWellConnector airWellConnector;
        private readonly TibberConnector tibberConnector;
        private readonly HyconConnector hyconConnector;
        private readonly AcquaNetConnector acquaNetConnector;

        private readonly FileDatabase<DateModel> database;

        private readonly int KlimaatSysteem = 7;
        private readonly int PompZwembadWarmtePomp = 8;
        private readonly int ZwembadWarmtePomp = 6;
        private readonly int ZwembadKlepOpen = 4;
        private readonly int ZwembadKlepDicht = 5;
        private readonly int LegionellaBoiler = 1;
        private readonly int BoilerKlepOpen = 2;
        private readonly int BoilerKlepDicht = 3;
        private readonly double BufferRangeZwembad = 0.4;

        public ZwembadService(RelayConnector relayConnector, AirWellConnector airWellConnector, TibberConnector tibberConnector, HyconConnector hyconConnector, AcquaNetConnector acquaNetConnector)
        {
            this.relayConnector = relayConnector;
            this.airWellConnector = airWellConnector;
            this.tibberConnector = tibberConnector;
            this.hyconConnector = hyconConnector;
            this.acquaNetConnector = acquaNetConnector;

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

        public async Task GetDataAsync()
        {
            var priceInfo = await tibberConnector.GetPriceLevel();
            var airWellData = await airWellConnector.GetDataAsync();
            var hyconData = await hyconConnector.GetDataAsync();

            PriceLevel priceLevel = CalculateCurrentPriceLevel(priceInfo.Today, priceInfo.Current).GetValueOrDefault(PriceLevel.Normal);

            CurrentState.Instance.TargetBoilerWaterTemp = airWellData.Airwell_MIDEA_CAC_Basic_0_hotWaterTemperature_0__0_7_0_last;
            CurrentState.Instance.CurrentBoilerWaterTemp = airWellData.Airwell_MIDEA_CAC_Basic_0_waterTankTemperature_0__0_21_0_last;
            CurrentState.Instance.TargetBufferWaterTemp = airWellData.Airwell_MIDEA_CAC_Basic_0_temperatureZone1_0__0_5_0_last;
            CurrentState.Instance.CurrentPriceLevel = priceLevel.ToString();
            CurrentState.Instance.TargetZwembadWaterTemp = hyconData.TargetTempature;
            CurrentState.Instance.CurrentZwembadWaterTemp = hyconData.CurrentTempature;
            CurrentState.Instance.TargetZwembadWaterPH = hyconData.TargetPh;
            CurrentState.Instance.CurrentZwembadWaterPH = hyconData.CurrentPh;
            CurrentState.Instance.TargetZwembadWaterChloor = hyconData.TargetChloor;
            CurrentState.Instance.CurrentZwembadWaterChloor = hyconData.CurrentChloor;
            CurrentState.Instance.TargetZwembadWaterFlow = hyconData.TargetFlow;
            CurrentState.Instance.CurrentZwembadWaterFlow = hyconData.CurrentFlow;
            CurrentState.Instance.currentDateTime = DateTime.UtcNow;



            await ExecuteChangeAsync(priceInfo.Current.Total ?? default, priceLevel, airWellData, hyconData);



            /////////////////////////////////////////Klimaat Systeem////////////////////////////////////////////////////////////////////////
            if(CurrentState.Instance.klimaatMode == "auto")
            {
                if (KlimaatSysteemMoetAan(priceInfo.Today, priceInfo.Current))
                {
                    await StartKlimaatSysteemasync();
                }
                else
                {
                    await StopKlimaatSysteemasync();
                }
            }

            /////////////////////////////////////////Automatisch Spoelen////////////////////////////////////////////////////////////////////////
            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                var priceLow = decimal.MaxValue;
                var time = string.Empty;

                for (int i = 0; i < priceInfo.Today.Count - 3; i++) 
                {
                    var price = priceInfo.Today.ElementAt(i);

                    var start = DateTime.Parse(price.StartsAt);
                    var calculatingPrice = price.Total + priceInfo.Today.ElementAt(i+1).Total + priceInfo.Today.ElementAt(i+2).Total;
                    if (calculatingPrice < priceLow)
                    {
                        priceLow = (decimal)calculatingPrice;
                        time = price.StartsAt;
                    }
                }

                if (priceInfo.Current.StartsAt == time)
                {
                    await StartLegionellasync();
                }
            }
        }


        private PriceLevel? CalculateCurrentPriceLevel(ICollection<Price> prices, Price currentPrice)
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

        private bool KlimaatSysteemMoetAan(ICollection<Price> prices, Price currentPrice)
        {
            var startFrame = new TimeOnly(22,0,0);
            var endFrame = new TimeOnly(5,0,0);
            var priceLow = decimal.MaxValue;
            var time = string.Empty;

            foreach (var price in prices)
            {
                var start = DateTime.Parse(price.StartsAt);
                if(start.Hour >= startFrame.Hour || start.Hour <= endFrame.Hour)
                {
                    if(price.Total < priceLow)
                    {
                        priceLow = (decimal)price.Total;
                        time = price.StartsAt;
                    }
                }
            }

            if (DateTime.Now.DayOfWeek == DayOfWeek.Saturday)
            {
                startFrame = new TimeOnly(14, 0, 0);
            }

            if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
            {
                startFrame = new TimeOnly(6, 0, 0);
            }

            var currentTime = DateTime.Parse(currentPrice.StartsAt);
            return currentPrice.StartsAt == time || (currentTime.Hour < startFrame.Hour && currentTime.Hour > endFrame.Hour);
        }

        private async Task ExecuteChangeAsync(decimal totalPrice, PriceLevel priceLevel, AirWellData airWellData, HyconData hyconData)
        {
            ///////////////////////////////////////Legionella mode////////////////////////////////////////////////////////////////////////
            if (CurrentState.Instance.LegionellaBoiler)
            {
                if(CurrentState.Instance.CurrentBoilerWaterTemp >= 64)
                {
                    CurrentState.Instance.LegionellaBoiler = false;
                    CurrentState.Instance.Spoelen = "uit";
                    await acquaNetConnector.StartSpoelenAsync();
                    relayConnector.OpenRelay(LegionellaBoiler);
                }
            }

            ///////////////////////////////////////Boiler Klep////////////////////////////////////////////////////////////////////////
            if (CurrentState.Instance.boilerMode == "auto")
            {
                if (priceLevel == PriceLevel.Expensive || priceLevel == PriceLevel.VeryExpensive)
                {
                    if (CurrentState.Instance.CurrentBoilerWaterTemp >= 50)
                    {
                        await CloseBoilerKlepAsync();
                    }
                    else
                    {
                        await OpenBoilerKlepAsync();
                    }
                }
                else
                {
                    if (CurrentState.Instance.CurrentBoilerWaterTemp >= 50)
                    {
                        await CloseBoilerKlepAsync();
                    }
                    else
                    {
                        await OpenBoilerKlepAsync();
                    }
                }
            }


            ///////////////////////////////////////Airwell Warmte Pomp////////////////////////////////////////////////////////////////////////
            if (CurrentState.Instance.airwellMode == "auto")
            {
                if (priceLevel == PriceLevel.Expensive || priceLevel == PriceLevel.VeryExpensive)
                {
                    if (CurrentState.Instance.CurrentBoilerWaterTemp < 40)
                    {
                        await StartAirwellWarmtePompasync();
                        await SetNormalTempAirwellWarmtePompasync();
                    }
                    else
                    {
                        //await StopAirwellWarmtePompasync();
                        await SetLowTempAirwellWarmtePompasync();
                    }
                }
                else if (priceLevel == PriceLevel.Normal)
                {
                    await StartAirwellWarmtePompasync();
                    await SetNormalTempAirwellWarmtePompasync();
                }
                else
                {
                    await StartAirwellWarmtePompasync();
                    await SetHighTempAirwellWarmtePompasync();
                }
            }
            ///////////////////////////////////////Zwembad temperature////////////////////////////////////////////////////////////////////////
            if (priceLevel == PriceLevel.Expensive || priceLevel == PriceLevel.VeryExpensive)
            {

                if (hyconData.CurrentTempature < (hyconData.TargetTempature - (2 * BufferRangeZwembad)))
                {
                    await OpenZwembadKlepAsync();
                }else
                {
                    await CloseZwembadKlepAsync();
                }


                if (hyconData.CurrentTempature < (hyconData.TargetTempature - BufferRangeZwembad))
                {
                    await StartZwembadWarmtePompasync();
                }
                else
                {
                    await StopZwembadWarmtePompasync();
                }
            }
            else if (priceLevel == PriceLevel.Normal)
            {
                if (hyconData.CurrentTempature < hyconData.TargetTempature)
                {
                    await StartZwembadWarmtePompasync();
                    await OpenZwembadKlepAsync();
                }
                else
                {
                    await CloseZwembadKlepAsync();
                    await StopZwembadWarmtePompasync();
                }
            }
            else
            {
                var additionalBuffer = totalPrice <= 0 ? 1 : 0;
                if (hyconData.CurrentTempature < (hyconData.TargetTempature + BufferRangeZwembad + additionalBuffer))
                {
                    await StartZwembadWarmtePompasync();
                    await OpenZwembadKlepAsync();
                }
                else
                {
                    await StopZwembadWarmtePompasync();
                    await CloseZwembadKlepAsync();

                }
            }

            if (CurrentState.Instance.CurrentBoilerWaterTemp < 48)
            {
                await CloseZwembadKlepAsync();
            }

            StoreState();
        }


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


        public async Task StartZwembadWarmtePompasync()
        {
            CurrentState.Instance.ZwembadWarmtePomp = true;
            relayConnector.CloseRelay(PompZwembadWarmtePomp);
            relayConnector.CloseRelay(ZwembadWarmtePomp);
        }

        public async Task StopZwembadWarmtePompasync()
        {
            CurrentState.Instance.ZwembadWarmtePomp = false;
            relayConnector.OpenRelay(ZwembadWarmtePomp);
            relayConnector.OpenRelay(PompZwembadWarmtePomp);
        }

        public async Task StartKlimaatSysteemasync()
        {
            CurrentState.Instance.KlimaatSysteem = true;
            relayConnector.CloseRelay(KlimaatSysteem);
        }

        public async Task StopKlimaatSysteemasync()
        {
            CurrentState.Instance.KlimaatSysteem = false;
            relayConnector.OpenRelay(KlimaatSysteem);
        }

        public async Task StartLegionellasync()
        {
            CurrentState.Instance.LegionellaBoiler = true;
            CurrentState.Instance.Spoelen = "aan";
            relayConnector.CloseRelay(LegionellaBoiler);
        }

        public async Task StopLegionellasync()
        {
            CurrentState.Instance.LegionellaBoiler = false;
            CurrentState.Instance.Spoelen = "uit";
            relayConnector.OpenRelay(LegionellaBoiler);
        }

        public async Task OpenBoilerKlepAsync()
        {
            if (CurrentState.Instance.BoilerKlepOpen != true)
            {
                CurrentState.Instance.BoilerKlepOpen = true;
                relayConnector.CloseRelay(BoilerKlepOpen);
                await Task.Delay(10000);
                relayConnector.OpenRelay(BoilerKlepOpen);
            }
        }

        public async Task CloseBoilerKlepAsync()
        {

            if (CurrentState.Instance.BoilerKlepOpen != false)
            {
                CurrentState.Instance.BoilerKlepOpen = false;

                relayConnector.CloseRelay(BoilerKlepDicht);
                await Task.Delay(10000);// full is 30 seconde
                relayConnector.OpenRelay(BoilerKlepDicht);
            }
        }

        public async Task OpenZwembadKlepAsync()
        {
            CurrentState.Instance.ZwembadKlepOpen = true;

            relayConnector.CloseRelay(ZwembadKlepOpen);
            relayConnector.CloseRelay(ZwembadKlepDicht);
        }

        public async Task CloseZwembadKlepAsync()
        {
            CurrentState.Instance.ZwembadKlepOpen = false;

            relayConnector.OpenRelay(ZwembadKlepOpen);
            relayConnector.OpenRelay(ZwembadKlepDicht);
        }
    }
}
