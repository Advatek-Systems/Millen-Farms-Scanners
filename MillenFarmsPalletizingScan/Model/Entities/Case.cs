using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Case
    {
        [Display(Name = "Serial Number")]
        public string SerialNo { get; set; }
    }
}
