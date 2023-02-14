using System.ComponentModel.DataAnnotations;

namespace InterviewTest.API.Models.Customer
{
    public class CreateCustomerRequest
    {
        [Required]
        public string Firstname { get; set; }
        [Required]
        public string Surname { get; set; }
    }
}
