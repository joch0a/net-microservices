using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text.Json;

namespace CommandsService.EventProccesing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopedFactory;
        private readonly IMapper _mapper;

        public EventProcessor(
            IServiceScopeFactory scopedFactory,
            IMapper mapper)
        {
            _scopedFactory = scopedFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    AddPlatform(message);
                    break;
                default:
                    break;
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("--> Determining event");

            var eventType = JsonSerializer.Deserialize<GenericEventDto>(notificationMessage);

            switch (eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("--> Platform published event detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("--> Could not determine event type");
                    return EventType.Undetermined;
            }
        }

        private void AddPlatform(string platformPublishedMessage)
        {
            using var scoped = _scopedFactory.CreateScope();
            var commandRepository = scoped.ServiceProvider.GetRequiredService<ICommandRepository>();
            var platformPublishedDto = JsonSerializer.Deserialize<PlatformPublishedDto>(platformPublishedMessage);

            try
            {
                var platform = _mapper.Map<Platform>(platformPublishedDto);

                if (commandRepository.ExternalPlatformExists(platform.ExternalId))
                {
                    Console.WriteLine("--> Platform already exists...");
                }
                else 
                {
                    commandRepository.CreatePlatform(platform);
                    commandRepository.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not add platform to the DB: {ex.Message}");
            }
        }
    }

    public enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}
