using PlatformService.Modles;

namespace PlatformService.Data 
{
    
    public class PlatformRepo : IPlatformRepo
    {

        private readonly AppDbContext _context;

        public PlatformRepo(AppDbContext context)
        {
            this._context = context;
        }
        public void CreatePlatrom(Platform plat)
        {
            if (plat == null)
            {
                throw new ArgumentNullException(nameof(plat));
            }

            _context.Add(plat);
        }

        public IEnumerable<Platform> GetAllPlatforms()
        {
            return _context.Platforms.ToList();
        }

        public Platform GetPlatformById(int Id)
        {
            return _context.Platforms.FirstOrDefault(p => p.Id == Id);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }
    }
}