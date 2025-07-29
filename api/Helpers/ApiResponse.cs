using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class ApiResponse<T>
    {
        public string Message { get; set; }
        public T Data { get; set; }


        public ApiResponse(string message, T data)
        {
            Message = message;
            Data = data;
        }
    }
}