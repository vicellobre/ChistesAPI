using Enjoy.Infrastructure.BackgroundJobs;
using Enjoy.Infrastructure.BackgroundJobs.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;

namespace Enjoy.Infrastructure;

public static partial class DependencyInjection
{
    public static IServiceCollection AddInfrastructureBackgroundJobs(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var outboxOptions = new OutboxProcessorOptions();
        configuration.GetSection(OutboxProcessorOptions.SectionName).Bind(outboxOptions);

        services.AddQuartz(quartz =>
        {
            string nameKey = nameof(ProcessOutboxMessagesJob);
            var jobKey = new JobKey(nameKey);
            var triggerKey = new TriggerKey($"{nameKey}-trigger");

            quartz.AddJob<ProcessOutboxMessagesJob>(job => job.WithIdentity(jobKey))
                .AddTrigger(trigger =>
                    trigger.ForJob(jobKey)
                        .WithIdentity(triggerKey)
                        .WithSimpleSchedule(schedule =>
                            schedule.WithIntervalInSeconds(outboxOptions.ProcessIntervalSeconds).RepeatForever()));
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }
}
