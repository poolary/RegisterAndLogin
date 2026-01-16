namespace LoggAutorz.Erros
{      public class ApiException
        {
            private string Message { get; set; } // Error message
            private string StatusCode { get; set; } // 404, 500, 200
            private string details { get; set; } // Additional details
            public ApiException(string _message, string _statusCode, string _details)
            {
                Message = _message;
                StatusCode = _statusCode;
                details = _details;
            }
        }
    }

