using AutoMapper;
using CommandsService.Data;
using CommandsService.Dtos;
using CommandsService.Models;
using Microsoft.AspNetCore.Mvc;

namespace CommandsService.Controllers
{

    [Route("api/c/platforms/{platformId}/[controller]")]
    [ApiController]
    public class CommandsController : ControllerBase
    {
        private ICommandRepo _commandRepo;
        private IMapper _mapper;

        public CommandsController(ICommandRepo commandRepo, IMapper mapper)
        {
            _commandRepo = commandRepo;
            _mapper = mapper;
        }

        [HttpGet]
        public ActionResult<IEnumerable<CommandReadDtos>> GetCommandsForPlatform(int platformId)
        {
            Console.WriteLine($"--> Hit GetCommandsForPlatforms {platformId}");

            if (!_commandRepo.PlatfromExists(platformId))
            {
                return NotFound();
            }
            var commands = _commandRepo.GetCommandsForPlatform(platformId);
            return Ok(_mapper.Map<IEnumerable<CommandReadDtos>>(commands));
        }
        [HttpGet("{commandId}", Name = "GetCommandForPlatform")]
        public ActionResult<CommandReadDtos> GetCommandForPlatform(int platformId, int commandId)
        {
            Console.WriteLine($"--> Hit CommandForPlatform: {platformId} / {commandId}");

            if (!_commandRepo.PlatfromExists(platformId)) 
            {
                return NotFound();
            }
            var command = _commandRepo.GetCommand(platformId, commandId);

            if (command == null) 
            {
                return NotFound();
            }

            return Ok(_mapper.Map<CommandReadDtos>(command));
        }

        [HttpPost(Name ="CreateCommandForPlatform")]
        public ActionResult<CommandReadDtos> CreateCommandForPlatform(int platformId, CommandCreateDtos commandCreateDtos)
        {
            Console.WriteLine($"--> Hit CreateCommandForPlatform: {platformId}");

            if (!_commandRepo.PlatfromExists(platformId))
            {
                return NotFound();
            }
            var command = _mapper.Map<Command>(commandCreateDtos);

            _commandRepo.CreateCommand(platformId, command);
            _commandRepo.SaveChanges();

            var commandReaDto = _mapper.Map<CommandReadDtos>(command);
            return CreatedAtRoute(nameof(CreateCommandForPlatform), new {platformId = platformId, commandId = commandReaDto.Id}, commandReaDto);

        }
    }
}