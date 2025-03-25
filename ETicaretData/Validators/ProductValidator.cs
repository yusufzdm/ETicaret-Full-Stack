using ETicaretData.Entities;
using FluentValidation;

namespace ETicaretData.Validators
{
    public class ProductValidator : AbstractValidator<Product>
    {
        public ProductValidator()
        {
            RuleFor(p => p.Name)
                .NotEmpty().WithMessage("Ürün adı boş bırakılamaz.")
                .Length(2, 100).WithMessage("Ürün adı 2-100 karakter arasında olmalıdır.");

            RuleFor(p => p.Description)
                .MaximumLength(500).WithMessage("Açıklama en fazla 500 karakter olabilir.");

            RuleFor(p => p.Price)
                .NotEmpty().WithMessage("Fiyat boş bırakılamaz.")
                .GreaterThan(0).WithMessage("Fiyat 0'dan büyük olmalıdır.");

            RuleFor(p => p.Stock)
                .NotEmpty().WithMessage("Stok boş bırakılamaz.")
                .GreaterThanOrEqualTo(0).WithMessage("Stok 0'dan küçük olamaz.");

            RuleFor(p => p.CategoryId)
                .NotEmpty().WithMessage("Kategori seçilmelidir.");

            RuleFor(p => p.Image)
                .NotEmpty().WithMessage("Ürün resmi boş bırakılamaz.");
        }
    }
} 