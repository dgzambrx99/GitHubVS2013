using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ProductionWIP.Models
{
    public class RegisterViewModelByAdmin
    {

        [Required]
        public string UserId { get; set; }
        [Required]
        [Display(Name = "UserName")]
        public string UserName { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} should have at least {2} characters.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and the confirmation don't match.")]
        public string ConfirmPassword { get; set; }

        //[Display(Name = "Email")]
        //[Required(ErrorMessage = "Enter the Email")]
        //[RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Ingrese un Correo Electrónico válido")]
        //public string Email { get; set; }

        //public string Nombre { get; set; }
        //public string Mensaje { get; set; }      
        //public string Role { get; set; }

    }
}