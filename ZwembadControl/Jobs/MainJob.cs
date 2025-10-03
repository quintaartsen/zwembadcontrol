using Quartz;
using ZwembadControl.Controllers;

namespace ZwembadControl.Jobs
{
    public class MainJob : IJob
    {
        private readonly ZwembadService _zwembadService;

        private bool isRunning = false;

        public MainJob(ZwembadService zwembadService)
        {
            _zwembadService = zwembadService;
        }

        async Task IJob.Execute(IJobExecutionContext context)
        {

            if (isRunning) { return; }

            isRunning = true;
            try
            {
                await _zwembadService.RunJob();
            }
            catch (Exception)
            {
                Console.WriteLine("Failed to handle main job");
            }
            isRunning = false;
        }
    }
}
