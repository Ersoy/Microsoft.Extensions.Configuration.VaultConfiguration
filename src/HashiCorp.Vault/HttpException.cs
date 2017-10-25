using System;
using System.Net;

namespace HashiCorp.Vault {

    public class HttpException : Exception {

        public HttpException(int httpStatusCode) {
            StatusCode = httpStatusCode;
        }

        public HttpException(HttpStatusCode httpStatusCode) {
            StatusCode = (int) httpStatusCode;
        }

        public HttpException(int httpStatusCode, string message) : base(message) {
            StatusCode = httpStatusCode;
        }

        public HttpException(HttpStatusCode httpStatusCode, string message) : base(message) {
            StatusCode = (int) httpStatusCode;
        }

        public HttpException(int httpStatusCode, string message, Exception innerException) : base(message, innerException) {
            StatusCode = httpStatusCode;
        }

        public HttpException(HttpStatusCode httpStatusCode, string message, Exception innerException) : base(message, innerException) {
            StatusCode = (int) httpStatusCode;
        }

        public HttpException() {
        }
        
        public int StatusCode { get; }
    }

}
