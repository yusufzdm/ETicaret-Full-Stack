using ETicaretData.Entities;
using FluentValidation;

namespace ETicaretData.Validators
{
    public class CategoryValidator : AbstractValidator<Category>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Kategori adı boş bırakılamaz.")
                .Length(2, 100).WithMessage("Kategori adı 2-100 karakter arasında olmalıdır.");

            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");
        }
    }
} 