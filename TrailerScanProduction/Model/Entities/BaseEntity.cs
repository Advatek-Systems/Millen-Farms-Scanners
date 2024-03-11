using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public abstract class BaseEntity
    {
        public List<ValidationError> Errors = new List<ValidationError>();

        public void AddError(ValidationError error)
        {
            Errors.Add(error);
        }
    }
}
