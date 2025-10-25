namespace Infrastructure.Http
{
    public class ApiResult<T>
    {
        public bool Success { get; set; }

        public string? ErrorMessage { get; set; }

        public T? Data { get; set; }
    }
}
