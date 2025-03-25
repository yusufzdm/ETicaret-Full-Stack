using ETicaretData.ViewModels;
using FluentValidation;

namespace ETicaretData.Validators
{
    public class ShippingDetailsValidator : AbstractValidator<ShippingDetails>
    {
        public ShippingDetailsValidator()
        {
            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("Kullanıcı adı boş bırakılamaz.")
                .Length(2, 50).WithMessage("Kullanıcı adı 2-50 karakter arasında olmalıdır.");

            RuleFor(x => x.AdressTitle)
                .NotEmpty().WithMessage("Adres başlığı boş bırakılamaz.")
                .Length(2, 50).WithMessage("Adres başlığı 2-50 karakter arasında olmalıdır.");

            RuleFor(x => x.Adress)
                .NotEmpty().WithMessage("Adres boş bırakılamaz.")
                .MaximumLength(500).WithMessage("Adres en fazla 500 karakter olabilir.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Şehir boş bırakılamaz.")
                .Length(2, 50).WithMessage("Şehir 2-50 karakter arasında olmalıdır.");
        }
    }
} 