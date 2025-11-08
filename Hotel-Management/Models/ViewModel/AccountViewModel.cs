using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models.ViewModel
{
    public class AccountViewModel
    {

        [Required(ErrorMessage = "Vui lòng nhập username")]
        [RegularExpression(@"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d_]+$", ErrorMessage = "Username cần có kí tự, số và có thể có dấu gạch dưới")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
