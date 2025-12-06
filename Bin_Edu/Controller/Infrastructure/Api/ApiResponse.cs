namespace Bin_Edu.Controller.Infrastructure.Api
{
    public class ApiResponse<T>
    {
        public string Message { get; set; }

        public T Data { get; set; }
    }
}
