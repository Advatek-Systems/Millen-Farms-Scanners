using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class BoxSize
    {
        [Display(Name = "Box Size ID")]
        public int BoxSizeID { get; set; }

        [Display(Name = "Box Size Name")]
        public string BoxSizeName { get; set; }
    }
}
