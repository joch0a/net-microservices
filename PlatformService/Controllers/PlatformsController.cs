using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;
        private readonly ICommandDataClient _commandDataClient;

        public PlatformsController(
            IPlatformRepository platformRepository, 
            IMapper mapper, 
            ICommandDataClient commandDataClient)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
            _commandDataClient = commandDataClient;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatforms()
        {
            return Ok(_mapper.Map<IEnumerable<PlatformReadDto>>(_platformRepository.GetAllPlatforms()));
        }

        [HttpGet("{id}", Name = "GetPlatformsById")]
        public ActionResult<IEnumerable<PlatformReadDto>> GetPlatformsById(int id)
        {
            var platform = _platformRepository.GetPlatformById(id);

            if (platform == null) 
            {
                return NotFound();
            }

            return Ok(_mapper.Map<PlatformReadDto>(platform));
        }

        [HttpPost]
        public async Task<ActionResult<PlatformReadDto>> CreatePlatformAsync(PlatformCreateDto platform) 
        {
            var platformModel = _mapper.Map<Platform>(platform);

            _platformRepository.CreatePlatform(platformModel);
            _platformRepository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            try
            {
                await _commandDataClient.SendPlatformToCommand(platformReadDto);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"--> Could not send synchronously: {ex.Message}");
            }

            return CreatedAtRoute(
                nameof(GetPlatformsById), 
                new { Id = platformReadDto.Id }, 
                platformReadDto);
        }
    }
}
