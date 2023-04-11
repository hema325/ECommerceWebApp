using DataAccess.DataAccessRepository.IRepository;

namespace ECommerceWebApp.Services.BackGroundServices
{
    public class TokensCleanUpHostedService : BackgroundService
    {
        private readonly IUnitOfWork UnitOfWork;

        public TokensCleanUpHostedService(IServiceProvider serviceProvider)
        {
            UnitOfWork = serviceProvider.CreateScope().ServiceProvider.GetService<IUnitOfWork>();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(TimeSpan.FromDays(1));
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await UnitOfWork.Tokens.CleanUpAsync();
            }
        }

    }
}
