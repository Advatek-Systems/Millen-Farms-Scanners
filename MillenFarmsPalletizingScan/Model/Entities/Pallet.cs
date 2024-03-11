using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Pallet
    {
        [Display(Name = "Pallet Number")]
        public long PalletID { get; set; }
        [Display(Name = "Started At")]
        [DisplayFormat(DataFormatString = "{0:f}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; }
    }
}
