using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Shipment : BaseEntity
    {
        public int ShipmentID { get; set; }

        [Display(Name = "Trailer Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Trailer Number is required.")]
        [StringLength(maximumLength:20, ErrorMessage = "Trailer Number cannot be more than 20 characters.")]
        public string TrailerNo { get; set; }

        public bool Completed { get; set; }

        [Display(Name = "Started At")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime StartedAt { get; set; }

        [Display(Name = "Completed At")]
        public DateTime? CompletedAt { get; set; }

        public int customerID { get; set; }
    }
}
