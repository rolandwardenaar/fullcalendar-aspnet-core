using System.ComponentModel.DataAnnotations;

namespace fullcalendarcore.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Gebruikersnaam is verplicht")]
        [Display(Name = "Gebruikersnaam")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht")]
        [DataType(DataType.Password)]
        [Display(Name = "Wachtwoord")]
        public string Password { get; set; }

        [Display(Name = "Onthoud mij")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Gebruikersnaam is verplicht")]
        [Display(Name = "Gebruikersnaam")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Weergavenaam is verplicht")]
        [Display(Name = "Weergavenaam")]
        public string DisplayName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Wachtwoord is verplicht")]
        [StringLength(100, ErrorMessage = "Het {0} moet minimaal {2} tekens lang zijn.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Wachtwoord")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bevestig wachtwoord")]
        [Compare("Password", ErrorMessage = "Wachtwoord en bevestiging komen niet overeen.")]
        public string ConfirmPassword { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Huidig wachtwoord is verplicht")]
        [DataType(DataType.Password)]
        [Display(Name = "Huidig wachtwoord")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Nieuw wachtwoord is verplicht")]
        [StringLength(100, ErrorMessage = "Het {0} moet minimaal {2} tekens lang zijn.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Nieuw wachtwoord")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Bevestig nieuw wachtwoord")]
        [Compare("NewPassword", ErrorMessage = "Nieuw wachtwoord en bevestiging komen niet overeen.")]
        public string ConfirmPassword { get; set; }
    }

    public class UserManagementViewModel
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string Roles { get; set; }
    }
}
