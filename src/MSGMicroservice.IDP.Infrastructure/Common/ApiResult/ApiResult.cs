using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace MSGMicroservice.IDP.Infrastructure.Common.ApiResult
{
    public class ApiResult<T> : IActionResult
    {
        public ApiResult()
        {
        }

        [JsonConstructor]
        public ApiResult(bool isSucceeded, string message = null)
        {
            Message = message;
            IsSucceeded = isSucceeded;
        }

        public ApiResult(bool isSucceeded, T data, string message = null)
        {
            Data = data;
            Message = message;
            IsSucceeded = isSucceeded;
        }

        public bool IsSucceeded { get; set; }
        public string Message { get; set; }
        public T Data { get; }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(this);
            await objectResult.ExecuteResultAsync(context);
        }
    }
}