using System;
using System.Net;
public class APIResponse<T>
    {
        public HttpStatusCode Response { get; set; }

        public string Error { get; set; }

        public T Result { get; set; }

    internal string ToString()
    {
        throw new NotImplementedException();
    }
}