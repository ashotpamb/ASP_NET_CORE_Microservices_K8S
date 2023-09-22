using PlatformService.Modles;

namespace PlatformService.Data 
{
    public interface IPlatformRepo 
    {
        bool SaveChanges();
        IEnumerable<Platform> GetAllPlatforms();
        Platform GetPlatformById(int Id);
        void CreatePlatrom(Platform plat);
    }
}