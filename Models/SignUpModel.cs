using System.ComponentModel.DataAnnotations;

namespace ProjectManager.Models
{
    public class SignUpModel
    {
        [Required(ErrorMessage = "L'email est requis.")]
        [EmailAddress(ErrorMessage = "Email invalide.")]
        public string Email { get; set; } = "";

        [Required(ErrorMessage = "Le nom complet est requis.")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Le mot de passe est requis.")]
        [MinLength(6, ErrorMessage = "Le mot de passe doit contenir au moins 6 caractères.")]
        public string Password { get; set; } = "";

        [Required(ErrorMessage = "La confirmation du mot de passe est requise.")]
        [Compare("Password", ErrorMessage = "Les mots de passe ne correspondent pas.")]
        public string ConfirmPassword { get; set; } = "";
    }
}