using System;
using System.Net;
using System.Text.Json.Serialization;

namespace BlazorChat.Shared
{
    public class ServiceResult<T>
    {
        public T Data { get; set; }
        public Exception Exception { get; set; }
        public string Message { get; set; }
        public bool IsSuccess => Exception == null && !HasValidationError;
        public bool HasValidationError { get; set; }

        [JsonConstructor]
        public ServiceResult(T data, string message, bool hasValidationError = false)
        {
            Data = data;
            Message = message;
            Exception = null;
            HasValidationError = hasValidationError;
        }

        public ServiceResult(Exception exception, string message)
        {
            Exception = exception;
            Message = message;
            Data = default;
        }
    }

    public class ApiResponseModel
    {
        public bool Success { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public Exception Exception { get; set; }
        public dynamic Data { get; set; }
        public ApiResponseModel(HttpStatusCode code, string message, Exception exception, dynamic data = null)
        {
            Success = (int)code >= 200 && (int)code < 300;
            StatusCode = code;
            Message = message;
            Exception = exception;
            Data = data;
        }
    }



}
