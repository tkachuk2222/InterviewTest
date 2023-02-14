namespace InterviewTest.API.Models
{
    public class ApiResponse<T>
    {
        public T Result { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
    }
}