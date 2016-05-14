using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace BiscuitChief.Models
{
    public class ContactUs
    {
        [Display(Name = "Email Address:")]
        [Required(ErrorMessage = "Please enter an Email Address")]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*", ErrorMessage = "Invalid Email Address")]
        public string EmailAddress { get; set; }

        [Display(Name = "Name:")]
        [Required(ErrorMessage = "Please enter your Name")]
        public string FullName { get; set; }

        [Display(Name = "Subject:")]
        [Required(ErrorMessage = "Please enter a Subject")]
        public string Subject { get; set; }

        [Display(Name = "Message:")]
        [Required(ErrorMessage = "Please enter a Message")]
        public string Message { get; set; }
    }
}