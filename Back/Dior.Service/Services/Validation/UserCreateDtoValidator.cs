using FluentValidation;
using Dior.Library.DTO;

namespace Dior.Service.Services.Validation
{
    public class UserCreateDtoValidator : AbstractValidator<UserCreateDto>
    {
        public UserCreateDtoValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Le nom d'utilisateur est obligatoire")
                .Length(2, 100).WithMessage("Le nom d'utilisateur doit contenir entre 2 et 100 caractères")
                .Matches("^[a-zA-Z0-9._-]+$").WithMessage("Le nom d'utilisateur ne peut contenir que des lettres, chiffres, points, tirets et underscores");

            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Le prénom est obligatoire")
                .Length(1, 100).WithMessage("Le prénom doit contenir entre 1 et 100 caractères");

            RuleFor(x => x.LastName)
                .NotEmpty().WithMessage("Le nom de famille est obligatoire")
                .Length(1, 100).WithMessage("Le nom de famille doit contenir entre 1 et 100 caractères");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("L'email est obligatoire")
                .EmailAddress().WithMessage("L'email n'est pas valide")
                .Length(1, 255).WithMessage("L'email doit contenir entre 1 et 255 caractères");

            RuleFor(x => x.Phone)
                .Matches(@"^\+?[1-9]\d{1,14}$").WithMessage("Le format du téléphone n'est pas valide")
                .When(x => !string.IsNullOrWhiteSpace(x.Phone));

            RuleFor(x => x.Password)
                .MinimumLength(8).WithMessage("Le mot de passe doit contenir au moins 8 caractères")
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)").WithMessage("Le mot de passe doit contenir au moins une minuscule, une majuscule et un chiffre")
                .When(x => !string.IsNullOrWhiteSpace(x.Password));

            RuleFor(x => x.TeamId)
                .GreaterThan(0).WithMessage("L'ID de l'équipe doit être positif")
                .When(x => x.TeamId.HasValue);

            RuleForEach(x => x.RoleIds)
                .GreaterThan(0).WithMessage("Les IDs de rôles doivent être positifs");
        }
    }
}