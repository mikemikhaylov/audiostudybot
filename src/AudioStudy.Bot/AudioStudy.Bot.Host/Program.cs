using AudioStudy.Bot.Application;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.DataAccess.Telegram;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services;
using AudioStudy.Bot.Domain.Services.Queue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AudioStudy.Bot.Host
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddOptions<TelegramOptions>()
                        .Bind(hostContext.Configuration.GetSection("Telegram")).ValidateDataAnnotations();
                    services.AddOptions<UpdatesGetterOptions>()
                        .Bind(hostContext.Configuration.GetSection("UpdatesGetterHostedService")).ValidateDataAnnotations();
                    services.AddOptions<QueueOptions>()
                        .Bind(hostContext.Configuration.GetSection("Queue")).ValidateDataAnnotations();
                    
                    services.AddSingleton<ITelegramClient, TelegramClient>();
                    services.AddSingleton<UpdatesQueue<TelegramUpdate>>(); // We must explicitly register Foo
                    services.AddSingleton<IUpdatesQueuePublisher<TelegramUpdate>>(x => x.GetRequiredService<UpdatesQueue<TelegramUpdate>>()); 
                    services.AddSingleton<IUpdatesQueueSubscriber<TelegramUpdate>>(x => x.GetRequiredService<UpdatesQueue<TelegramUpdate>>()); 
                    services.AddHostedService<TelegramPipelineHostedService>();
                    services.AddHostedService<UpdatesGetterHostedService>();
                });
    }
}