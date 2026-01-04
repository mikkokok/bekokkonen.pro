using bekokkonen.pro.MQ.Implementation;

namespace bekokkonen.pro.MQ
{
    public class MQWorker(ILogger<MQWorker> logger, MQClient mQClient) : BackgroundService
    {
        private readonly ILogger<MQWorker> _logger = logger;
        private readonly MQClient _mQClient = mQClient;
        private readonly string _serviceName = nameof(MQWorker);

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("{ServiceName}:: Started", _serviceName);
            try
            {
                var mqTask = _mQClient.Initialization;
                if (mqTask.Exception != null)
                {
                    _logger.LogCritical(mqTask.Exception, "{ServiceName}:: Error in MQClient task", _serviceName);
                }

                await Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "{ServiceName}:: Encountered error", _serviceName);
            }
        }
    }
}
