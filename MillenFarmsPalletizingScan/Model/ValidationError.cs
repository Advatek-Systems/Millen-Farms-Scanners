using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ValidationError
    {
        public ValidationError(string desc)
        {
            Description = desc;
        }

        public string Description { get; set; }
    }
}
