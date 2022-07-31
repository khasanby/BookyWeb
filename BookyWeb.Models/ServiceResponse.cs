using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookyWeb.Models
{
    public class ServiceResponse<T>
    {
        public T Data { get; set; }
        public bool Status { get; set; } = true;
        public string Message { get; set; } = string.Empty;
    }
}
