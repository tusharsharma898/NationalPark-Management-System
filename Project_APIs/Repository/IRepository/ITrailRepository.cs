using Project_APIs.Models;

namespace Project_APIs.Repository.IRepository
{
    public interface ITrailRepository
    {
        ICollection<Trail> GetTrails();
        Trail GetTrail(int trailId);
        bool TrailExists(string trailName);
        bool TrailExists(int trailId);
        bool CreateTrail(Trail trail);
        bool UpdateTrail(Trail trail);
        bool DeleteTrail(Trail trail);
        bool Save();
        ICollection<Trail> GetTrailsInNationalPark(int nationalParkId);
    }
}
