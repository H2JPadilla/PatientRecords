using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EL
{
    public class PatientEntity : Entity
    {
        public int ID { get; set; }

        [Required(ErrorMessage = "Patient name is required.")]
        public string Patient { get; set; }

        [Required(ErrorMessage = "Drug name is required.")]
        public string DrugName { get; set; }

        [Required(ErrorMessage = "Dosage is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Dosage must be a positive number.")]
        public int Dosage { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}