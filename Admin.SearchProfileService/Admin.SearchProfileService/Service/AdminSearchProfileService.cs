using System;
using System.Threading;
using System.Threading.Tasks;
using Admin.SearchProfileService.Kafka;
using Admin.SearchProfileService.Model;
using Admin.SearchProfileService.Repository.Contracts;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Admin.SearchProfileService.Service
{
    public class AdminSearchProfileService : BackgroundService
    {

        public AdminSearchProfileService(IServiceProvider services)
        {
            Services = services;
        }

        public IServiceProvider Services { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine(
                "AdminSearchProfileService: Consume Scoped Service Hosted Service running.");

            await StartConsume(stoppingToken);
        }

        private async Task StartConsume(CancellationToken stoppingToken)
        {
            Console.WriteLine(
                "AdminSearchProfileService: Consume Scoped Service Hosted Service is working.");

            using (var scope = Services.CreateScope())
            {
                var scopedProcessingService =
                    scope.ServiceProvider
                        .GetRequiredService<IScopedProcessingService>();

                await scopedProcessingService.StartConsume(stoppingToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            Console.WriteLine(
                "AdminSearchProfileService: Consume Scoped Service Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }

    internal interface IScopedProcessingService
    {
        Task StartConsume(CancellationToken stoppingToken);
    }

    internal class ScopedProcessingService : IScopedProcessingService
    {
        private int executionCount = 0;

        private readonly ISearchProfileRepository _repo;
        private readonly ConsumerConfig consumerConfig;
        private readonly ILogger<AdminSearchProfileService> _logger;
        private readonly ConsumerWrapper _consumerForInsetUserProfile;
        private readonly ConsumerWrapper _consumerForUpdateUserProfile;

        public ScopedProcessingService(ConsumerConfig consumerConfig, ILogger<AdminSearchProfileService> logger, ISearchProfileRepository repo)
        {
            _repo = repo;
            this.consumerConfig = consumerConfig;
            _logger = logger;
            _consumerForInsetUserProfile = new ConsumerWrapper(consumerConfig, "profileforadminaddtopic");
            _consumerForUpdateUserProfile = new ConsumerWrapper(consumerConfig, "profileforadminupdatetopic");
        }

        public async Task StartConsume(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                executionCount++;

                Console.WriteLine(
                   "AdminSearchProfileService: Scoped Processing Service is working. Count: {0}", executionCount);

                #region Insert-UserProfile (Admin-Service)
                string insertUserRequest = _consumerForInsetUserProfile.ReadMessage();
                if (!string.IsNullOrWhiteSpace(insertUserRequest))
                {
                    //Deserilaize 
                    UserProfile userProfileForInsert = JsonConvert.DeserializeObject<UserProfile>(insertUserRequest);
                    try
                    {
                        await _repo.InsertUserProfileRepository(userProfileForInsert);
                        string message = "Status Code:200, Message:UserProfile Record inseterd successfully, Operation:AdminSearchProfileService consumer.";
                        _logger.LogInformation("{date} : InsertUserProfile of AdminSearchProfileService consumer operation executed. Message:{message}.", DateTime.UtcNow, message);
                        Console.WriteLine(message);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An unknown error occurred on the InsertUserProfile of AdminSearchProfileService consumer operation.");
                        Console.WriteLine("Status Code:0,  Message:Unknown Exception Occured, Operation:AdminSearchProfileService consumer, Exception:" + ex);
                    }
                }
                #endregion

                #region Update-UserProfile (Admin-Service)
                string updateUserRequest = _consumerForUpdateUserProfile.ReadMessage();
                if (!string.IsNullOrWhiteSpace(updateUserRequest))
                {
                    UserProfile userProfileForUpdate = JsonConvert.DeserializeObject<UserProfile>(updateUserRequest);
                    try
                    {
                        bool updateStatus = await _repo.UpdateUserProfileRepository(userProfileForUpdate);

                        if (updateStatus)
                        {
                            string message = "Status Code:200, Message:UserProfile Record updated successfully, Operation:AdminSearchProfileService consumer.";
                            _logger.LogInformation("{0} : UpdateUserProfile of AdminSearchProfileService consumer operation executed. Message:{1}.", DateTime.UtcNow, message);
                            Console.WriteLine(message);
                        }
                        else
                        {
                            string message = "Status Code:405, Message:UserProfile Record update failed, Operation:AdminSearchProfileService consumer.";
                            _logger.LogInformation("{date} : UpdateUserProfile of AdminSearchProfileService consumer operation executed. Message:{message}.", DateTime.UtcNow, message);
                            Console.WriteLine(message);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "An unknown error occurred on the UpdateUserProfile of AdminSearchProfileService consumer operation.");
                        Console.WriteLine("Status Code:0, Message:Unknown Exception Occured, Operation:AdminSearchProfileService consumer, Exception:" + ex);
                    }
                }
                #endregion

                await Task.Delay(100, stoppingToken);
            }
        }
    }
}