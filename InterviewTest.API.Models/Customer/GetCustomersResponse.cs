namespace InterviewTest.API.Models.Customer
{
    public class GetCustomersResponse
    {
        public int TotalCount { get; set; } = 0;
        public IEnumerable<GetCustomerItem> Customers { get; set; } = new List<GetCustomerItem>();
    }

    public class GetCustomerItem
    {
        public Guid Id { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
    }
}