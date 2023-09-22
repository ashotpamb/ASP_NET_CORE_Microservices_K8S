using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{
    [Route("api/c/[controller]")]
    [ApiController]
    public class PlatformsController : ControllerBase
    {
        private readonly ICommandRepo _commandRepo;
        private readonly IMapper _mapper;

        public PlatformsController(ICommandRepo commandRepo, IMapper mapper)
        {
            _commandRepo = commandRepo;
            _mapper  = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PlatformReadDtos>> GetPlatforms()
        {
            Console.WriteLine("--> Getting Platforms from Command Service");
            var platforms = _commandRepo.GetAllPlatforms();
            return Ok(_mapper.Map<IEnumerable<PlatformReadDtos>>(platforms));

        } 

        [HttpPost]
        public ActionResult TestConnection()
        {
            Console.WriteLine("--> Inbount POST # Command Service");
            return Ok("Inbount test from Platform Controller");
        }
    }
}