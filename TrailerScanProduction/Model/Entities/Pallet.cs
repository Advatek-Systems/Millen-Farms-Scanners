using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Pallet : BaseEntity
    {
        public int PalletID { get; set; }

        [Display(Name = "Pallet Number")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Pallet Number is required.")]
        [StringLength(maximumLength:20, ErrorMessage = "Pallet Number cannot be more than 20 characters.")]
        public string PalletNo { get; set; }

        public int ShipmentID { get; set; }

        [Display(Name = "Scanned At")]
        public DateTime ScannedAt { get; set; }

        public string ReadyToPrint { get; set; }

        public string IsPalletized { get; set; }

        public int customerID { get; set; }
    }
}
