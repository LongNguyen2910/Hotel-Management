using System.ComponentModel.DataAnnotations;

namespace Hotel_Management.Models.ViewModel
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập username")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mật khẩu để đăng ký")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
