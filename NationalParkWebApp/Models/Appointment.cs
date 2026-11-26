
using System;
using System.ComponentModel.DataAnnotations;

namespace NationalParkWebApp.Models
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

        [Required]
        public int NationalParkId { get; set; }

        [Range(0, 5)]
        public int AdultCount { get; set; }

        [Range(0, 5)]
        public int ChildCount { get; set; }

        public string? TransactionId { get; set; }

        public string Status { get; set; } = "Pending";
    }
}
