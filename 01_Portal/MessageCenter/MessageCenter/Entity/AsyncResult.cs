using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MessageCenter.Entity
{
    public class AsyncResult<T>
    {
        public T Result { get; set; }

        public Exception Error { get; set; }
    }
}
