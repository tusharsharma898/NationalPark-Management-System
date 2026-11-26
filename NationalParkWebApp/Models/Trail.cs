using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NationalParkWebApp.Models
{
    public class Trail
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Distance { get; set; }
        public string Elevation { get; set; }
        public enum DifficultyType { Easy, Moderate, Difficult }
        public DifficultyType Difficulty { get; set; }
        [Display(Name ="National Park")]
        public int NationalParkId { get; set; }
        public NationalPark NationalPark { get; set; }
    }
}
