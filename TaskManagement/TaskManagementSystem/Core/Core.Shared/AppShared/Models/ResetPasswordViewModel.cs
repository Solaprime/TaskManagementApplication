using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AppShared.Models
{
   public class ResetPasswordViewModel
    {
        [EmailAddress]
        [StringLength(50)]
        public string Email { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 5)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        [DataType(DataType.Password)]
        [Display(Name = "ConfirmPassword")]
        [Compare("Password",
            ErrorMessage = " confirm Password and Paassword dont match")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string Token { get; set; }

    }
}
