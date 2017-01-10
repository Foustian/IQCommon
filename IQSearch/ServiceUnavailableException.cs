using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IQSearch
{
    public class ServiceUnavailableException : System.ApplicationException
    {
            public ServiceUnavailableException() { }
            public ServiceUnavailableException(string message) { }
            public ServiceUnavailableException(string message, System.Exception inner) { }

            // Constructor for serialization 
            protected ServiceUnavailableException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) { }
        }
    }

