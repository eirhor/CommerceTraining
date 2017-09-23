using System.ComponentModel.DataAnnotations;

namespace CommerceFundamentalsWeb.Models.ViewModels
{
    public class AccountViewModel
    {
        public string UserName { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}