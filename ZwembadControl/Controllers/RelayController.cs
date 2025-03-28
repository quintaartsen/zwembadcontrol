using Microsoft.AspNetCore.Mvc;
using ZwembadControl.Connectors;

namespace ZwembadControl.Controllers
{
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class RelayController : Controller
    {
        private readonly RelayConnector _controller;

        public RelayController(RelayConnector controller)
        {
            _controller = controller;
        }

        [HttpPost]
        [Route("OpenRelay")]
        public void OpenRelay(int pin)
        {
            _controller.OpenRelay(pin);
        }

        [HttpPost]
        [Route("CloseRelay")]
        public void CloseRelay(int pin)
        {
            _controller.CloseRelay(pin);
        }
    }
}
