using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using WebApp.ApiClient;

namespace Infrastructure.Http
{
    public class ApiClientService
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly JsonSerializerOptions jsonOptions;

        public ApiClientService(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;

            this.jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
            };
            this.jsonOptions.Converters.Add(new JsonStringEnumConverter());
        }

        public async Task<ApiResult<T>> TryRequestAsync<T>(Func<Task<HttpResponseMessage>> request)
        {
            try
            {
                ArgumentNullException.ThrowIfNull(request);

                var response = await request();

                if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    var context = this.httpContextAccessor.HttpContext;
                    context?.Response.Cookies.Delete("jwt");

                    if (context != null && !context.Response.HasStarted)
                    {
                        context.Response.Redirect("/User/Login");
                    }

                    return new ApiResult<T>
                    {
                        Success = false,
                        ErrorMessage = "Unauthorized: please login again.",
                    };
                }

                if (response.IsSuccessStatusCode)
                {
                    if (response.Content.Headers.ContentLength == 0)
                    {
                        return new ApiResult<T> { Success = true };
                    }

                    var data = await response.Content.ReadFromJsonAsync<T>(this.jsonOptions);
                    return new ApiResult<T> { Success = true, Data = data };
                }

                var content = await response.Content.ReadAsStringAsync();
                var message = ExtractErrorMessage(content);

                return new ApiResult<T> { Success = false, ErrorMessage = message };
            }
            catch (HttpRequestException ex)
            {
                return new ApiResult<T> { Success = false, ErrorMessage = $"Network error: {ex.Message}" };
            }
            catch (TaskCanceledException)
            {
                return new ApiResult<T> { Success = false, ErrorMessage = "The server timed out waiting for a response." };
            }
            catch (JsonException ex)
            {
                return new ApiResult<T> { Success = false, ErrorMessage = $"JSON parsing error: {ex.Message}" };
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Error executing request.", ex);
            }
        }

        private static string ExtractErrorMessage(string content)
        {
            try
            {
                var doc = JsonDocument.Parse(content);
                if (doc.RootElement.ValueKind == JsonValueKind.Object)
                {
                    var firstValue = doc.RootElement.EnumerateObject().FirstOrDefault().Value;
                    return firstValue.ToString();
                }
            }
            catch (JsonException)
            {
                // Ignore JSON parsing errors and return raw content below
            }

            return content;
        }
    }
}
