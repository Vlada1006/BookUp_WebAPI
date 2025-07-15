using api.Interfaces;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Routing.Tree;

namespace api.Services
{
    public class OldUserAccountCleanUpService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly TimeSpan _interval = TimeSpan.FromHours(24);
        public OldUserAccountCleanUpService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DoCleanuUpAsync();
                await Task.Delay(_interval, stoppingToken);
            }
        }

        private async Task DoCleanuUpAsync()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
                var users = userManager.Users.ToList();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

                foreach (var user in users)
                {
                    var now = DateTime.UtcNow;


                    if (user.LastLoginDate.HasValue && user.LastLoginDate.Value.AddYears(3) < now
                            && user.DeletionWarningSent == null)
                    {
                        await emailService.SendEmailAsync(
                            user.Email,
                            "Account Deletion Warning",
                            $"Hello, {user.UserName},<br>Your account has been inactive for 3 years. It will be deleted in 30 days unless you log in."
                        );

                        user.DeletionWarningSent = now;
                        await userManager.UpdateAsync(user);
                    }

                    if (user.LastLoginDate.HasValue && user.LastLoginDate.Value.AddYears(3) < now
                            && user.DeletionWarningSent.HasValue && user.DeletionWarningSent.Value.AddDays(30) < now)
                    {
                        await userManager.DeleteAsync(user);

                        await emailService.SendEmailAsync(
                            user.Email,
                            "Account Deleted",
                            $"Hello, {user.UserName},<br>Your account has been inactive for more than 3 years. /nFor safety reasons it has been deleted. Register again next time you use our app."
                        );
                    }

                    if (!user.LastLoginDate.HasValue)
                    {
                        await userManager.DeleteAsync(user);
                    }
                }
            }
        }
    }
}


