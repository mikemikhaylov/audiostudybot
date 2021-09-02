using System;
using System.Collections.Generic;
using AudioStudy.Bot.Analytics;
using AudioStudy.Bot.Application;
using AudioStudy.Bot.DataAccess.Abstractions;
using AudioStudy.Bot.DataAccess.Db;
using AudioStudy.Bot.DataAccess.Telegram;
using AudioStudy.Bot.Domain.Model.Courses;
using AudioStudy.Bot.Domain.Model.Telegram;
using AudioStudy.Bot.Domain.Services;
using AudioStudy.Bot.Domain.Services.Courses;
using AudioStudy.Bot.Domain.Services.Telegram;
using AudioStudy.Bot.Domain.Services.Telegram.Helpers;
using AudioStudy.Bot.Domain.Services.Telegram.Middlewares;
using AudioStudy.Bot.Domain.Services.Telegram.Middlewares.MenuSubMiddlewares;
using AudioStudy.Bot.SharedUtils.Localization;
using AudioStudy.Bot.SharedUtils.Localization.LocalizationSource;
using AudioStudy.Bot.SharedUtils.Metrics;
using AudioStudy.Bot.SharedUtils.Queue;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Polly;

namespace AudioStudy.Bot.Host
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            host.Services.GetService<CourseProvider>()!.Load();
            host.Services.GetService<LessonProvider>()!.Load();
            host.Run();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Microsoft.Extensions.Hosting.Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddHttpClient(Options.DefaultName)
                        .AddTransientHttpErrorPolicy(p => 
                            p.WaitAndRetryAsync(2, _ => TimeSpan.FromMilliseconds(600)));
                    
                    services.AddOptions<TelegramOptions>()
                        .Bind(hostContext.Configuration.GetSection("Telegram")).ValidateDataAnnotations();
                    services.AddOptions<AnalyticsOptions>()
                        .Bind(hostContext.Configuration.GetSection("Analytics")).ValidateDataAnnotations();
                    services.AddOptions<UpdatesGetterOptions>()
                        .Bind(hostContext.Configuration.GetSection("UpdatesGetterHostedService"))
                        .ValidateDataAnnotations();
                    services.AddOptions<QueueOptions>()
                        .Bind(hostContext.Configuration.GetSection("Queue")).ValidateDataAnnotations();
                    services.AddOptions<DbOptions>()
                        .Bind(hostContext.Configuration.GetSection("Db")).ValidateDataAnnotations();

                    services.AddSingleton<MongoDbContext>();
                    services.AddSingleton<ILocalizationSource, JsonLocalizationSource>();
                    services.AddSingleton<IBotLocalization, BotLocalization>();
                    services.AddSingleton<ITelegramClient, TelegramClient>();
                    services.AddSingleton<IUserRepository, MongoDbUserRepository>();
                    services.AddSingleton<IUserService, UserService>();
                    services.AddSingleton<ITelegramButtonsHelper, TelegramButtonsHelper>();
                    services.AddSingleton<IFullCourseListPagingHelper, FullCourseListPagingHelper>();
                    services.AddSingleton<ICourseCardsPagingHelper, CourseCardsPagingHelper>();
                    services.AddSingleton<ILessonCardsPagingHelper, LessonCardsPagingHelper>();
                    services.AddSingleton<IFilterHelper, FilterHelper>();
                    services.AddSingleton<ICourseHelper, CourseHelper>();
                    services.AddSingleton<ILearnHelper, LearnHelper>();
                    services.AddSingleton<CourseProvider>();
                    services.AddSingleton<ICourseProvider, CourseProvider>();
                    services.AddSingleton<LessonProvider>();
                    services.AddSingleton<ILessonProvider, LessonProvider>();
                    services.AddSingleton<ChatTypeCheckerMiddleware>();
                    services.AddSingleton<UserContextProviderMiddleware>();
                    services.AddSingleton<CommandExecutorMiddleware>();
                    services.AddSingleton<SettingsCheckerMiddleware>();
                    services.AddSingleton<MenuMiddleware>();
                    services.AddSingleton<InlineKeyboardMiddleware>();
                    services.AddSingleton<IMenuSubMiddleware, RatingSubMiddleware>();
                    services.AddSingleton<IMenuSubMiddleware, MainWindowSubMiddleware>();
                    services.AddSingleton<IMenuSubMiddleware, SettingsSubMiddleware>();
                    services.AddSingleton<IMenuSubMiddleware, LanguageSettingSubMiddleware>();services.AddTransient<Func<IEnumerable<IMenuSubMiddleware>>>(provider =>
                        provider.GetServices<IMenuSubMiddleware>);
                    services.AddSingleton<IMenuSubMiddlewareFactory, MenuSubMiddlewareFactory>();
                    services.AddSingleton<ITelegramMessagePipeline, TelegramMessagePipeline>();
                    services.AddSingleton<UpdatesQueue<TelegramRequestMessage>>();
                    services.AddSingleton<IUpdatesQueuePublisher<TelegramRequestMessage>>(x =>
                        x.GetRequiredService<UpdatesQueue<TelegramRequestMessage>>());
                    services.AddSingleton<IUpdatesQueueSubscriber<TelegramRequestMessage>>(x =>
                        x.GetRequiredService<UpdatesQueue<TelegramRequestMessage>>());
                    services.AddHostedService<TelegramPipelineHostedService>();
                    services.AddHostedService<UpdatesGetterHostedService>();
                    services.AddSingleton<IAnalyticsQueue, AnalyticsQueue>();
                });
    }
}