namespace NationalParkWebApp.Models.ViewModels
{
    public class IndexVM
    {
        public IEnumerable<NationalPark> NationalParkList { get; set; } = new List<NationalPark>();
        public IEnumerable<Trail> TrailList { get; set; }
    }
}
