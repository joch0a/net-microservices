using CommandsService.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CommandsService.Data
{
    public class CommandRepository : ICommandRepository
    {
        private readonly AppDbContext _context;

        public CommandRepository(AppDbContext context)
        {
            _context = context;
        }

        public void CreateCommand(int platformId, Command command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }

            command.PlatformId = platformId;

            _context.Commands.Add(command);
        }

        public void CreatePlatform(Platform platform)
        {
            if (platform == null) 
            {
                throw new ArgumentNullException(nameof(platform));
            }

            _context.Platforms.Add(platform);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public Command GetCommand(int platformId, int commandId)
        {
            return _context.Commands
                .FirstOrDefault(platform => platform.Id == platformId);
        }

        public IEnumerable<Command> GetCommandsForPlatform(int platformId)
        {
            return _context.Commands
                .Where(platform => platform.Id == platformId)
                .OrderBy(command => command.Platform.Name);
        }

        public bool PlatformExists(int platformId)
        {
            return _context.Platforms.Any(platform => platform.Id == platformId);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() > 0);
        }
    }
}
