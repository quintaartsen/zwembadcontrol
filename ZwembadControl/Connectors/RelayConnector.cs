using System.Device.Gpio;

namespace ZwembadControl.Connectors
{
    public class RelayConnector
    {

        private readonly int[] pins = { 5, 6, 13, 16, 19, 20, 21, 26 };

        public void OpenRelay(int pin)
        {
            using var controller = new GpioController();
            {
                var controlPin = pins[pin - 1];

                controller.OpenPin(controlPin);
                controller.SetPinMode(controlPin, PinMode.Output);
                controller.Write(controlPin, PinValue.High);
            }
        }

        public void CloseRelay(int pin)
        {
            using var controller = new GpioController();
            {
                var controlPin = pins[pin - 1];

                controller.OpenPin(controlPin);
                controller.SetPinMode(controlPin, PinMode.Output);
                controller.Write(controlPin, PinValue.Low);
            }
        }
    }
}
