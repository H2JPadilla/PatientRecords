using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EL
{
    public class PatientEntity
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "All field/s are required.")]
        [StringLength(50, ErrorMessage = "Patient name cannot exceed 50 characters.")]
        public string Patient { get; set; }

        [Required(ErrorMessage = "All field/s are required.")]
        [StringLength(50, ErrorMessage = "Drug name cannot exceed 50 characters.")]
        public string DrugName { get; set; }
        [Required(ErrorMessage = "All field/s are required.")]
        [Range(0.0001, 99999999.9999, ErrorMessage = "Dosage must be a positive number up to 4 decimal places.")]
        [DisplayFormat(DataFormatString = "{0:0.0000}", ApplyFormatInEditMode = true)]
        public decimal Dosage { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}