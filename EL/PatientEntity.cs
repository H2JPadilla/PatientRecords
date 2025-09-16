using System;
using System.ComponentModel.DataAnnotations;

namespace EL
{
    public class PatientEntity
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Patient name is required.")]
        [MaxLength(50, ErrorMessage = "Patient name cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z\s'-]+$", ErrorMessage = "Patient name can only contain letters, spaces, hyphens (-), and apostrophes (').")]
        public string Patient { get; set; }

        [Required(ErrorMessage = "Drug name is required.")]
        [MaxLength(50, ErrorMessage = "Drug name cannot exceed 50 characters.")]
        [RegularExpression(@"^[a-zA-Z0-9\s]+$", ErrorMessage = "Drug name can only contain letters, numbers, and spaces.")]
        public string DrugName { get; set; }

        [Required(ErrorMessage = "Dosage is required.")]
        [Range(typeof(decimal), "0.01", "9999999.9999", ErrorMessage = "Dosage must be greater than 0 and up to 4 decimal places.")]
        public decimal Dosage { get; set; }

        public DateTime ModifiedDate { get; set; } = DateTime.Now;
    }
}
