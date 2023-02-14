namespace InterviewTest.API.Models.Customer
{
    public class GetCustomersRequest
    {
        public int Page { get; set; } = 0;
        public int Count { get; set; } = 2;
        public string SearchValue { get; set; } = "";
    }
}
