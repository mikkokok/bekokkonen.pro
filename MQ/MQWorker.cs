
using bekokkonen.pro.MQ.Implementation;

namespace bekokkonen.pro.MQ
{
    public class MQWorker : BackgroundService
    {
        private ILogger<MQWorker> _logger;
        private MQClient _mQClient;
        private readonly string _serviceName = nameof(MQWorker);

        public MQWorker(ILogger<MQWorker> logger, MQClient mQClient)
        {
            _logger = logger;
            _mQClient = mQClient;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{_serviceName}:: Started ");
            try
            {
                var mqTask = _mQClient.Initialization;
                if(mqTask.Exception != null)
                {
                    _logger.LogCritical(mqTask.Exception, $"{_serviceName}:: Error in MQClient task");
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, $"{_serviceName}:: Encountered error");
            }
        }
    }
}
