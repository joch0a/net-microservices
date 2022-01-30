using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PlatformService.Data;
using PlatformService.Dtos;
using PlatformService.Models;
using System.Collections.Generic;

namespace PlatformService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly IPlatformRepository _platformRepository;
        private readonly IMapper _mapper;

        public PlatformsController(IPlatformRepository platformRepository, IMapper mapper)
        {
            _platformRepository = platformRepository;
            _mapper = mapper;
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
        public ActionResult<PlatformReadDto> CreatePlatform(PlatformCreateDto platform) 
        {
            var platformModel = _mapper.Map<Platform>(platform);

            _platformRepository.CreatePlatform(platformModel);
            _platformRepository.SaveChanges();

            var platformReadDto = _mapper.Map<PlatformReadDto>(platformModel);

            return CreatedAtRoute(nameof(GetPlatformsById), new { Id = platformReadDto.Id}, platformReadDto);
        }
    }
}
