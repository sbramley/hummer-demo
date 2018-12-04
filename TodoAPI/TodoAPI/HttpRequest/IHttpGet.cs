using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TodoAPI.HttpRequest
{
    public interface IHttpGet
    {
        Task<string> GetFromURLAsync(string uri);
}
    }
