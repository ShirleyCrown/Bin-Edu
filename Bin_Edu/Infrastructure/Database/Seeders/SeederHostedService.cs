namespace Bin_Edu.Infrastructure.Database.Seeders
{
    public class SeederHostedService : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        public SeederHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var generator = scope.ServiceProvider.GetRequiredService<SeederRunner>();
            await generator.ExecuteGeneration();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    }
}
