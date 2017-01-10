using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace IQSearch
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string error, string url, WebException ex)
            : base(error, ex)
        {
            URL = url;
        }

        public string URL { get; private set; }
    }
}
