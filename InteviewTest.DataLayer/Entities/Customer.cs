namespace InterviewTest.DataLayer.Entities
{
    public class Customer : BaseEntity<Guid>
    {
        public string Firstname { get; set; }
        public string Surname { get; set; }
    }
}