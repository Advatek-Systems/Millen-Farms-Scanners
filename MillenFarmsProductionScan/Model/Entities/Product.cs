using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Product
    {
        [Display(Name = "Product ID")]
        public int ProductID { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; }
    }
}
