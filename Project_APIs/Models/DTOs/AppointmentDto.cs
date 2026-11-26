using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_APIs.Models.DTOs
{
    public class AppointmentDto
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime VisitDate { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        public int NationalParkId { get; set; }

        [ForeignKey(nameof(NationalParkId))]
        public NationalPark NationalPark { get; set; }

        [Range(0, 5)]
        public int AdultCount { get; set; }

        [Range(0, 5)]
        public int ChildCount { get; set; }

        public string? TransactionId { get; set; }

        public string Status { get; set; } = "Pending";
    }
}