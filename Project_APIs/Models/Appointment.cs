using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Project_APIs.Models
{
    public class Appointment
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
        [Range(0, 5)]
        public int AdultCount { get; set; }

        [Range(0, 5)]
        public int ChildCount { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal AdultPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal ChildPrice { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalPrice { get; set; }
        public string? TransactionId { get; set; }
        [Required]
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        [Required]
        public int NationalParkId { get; set; }

        [ForeignKey(nameof(NationalParkId))]
        public NationalPark NationalPark { get; set; }

    }
}
