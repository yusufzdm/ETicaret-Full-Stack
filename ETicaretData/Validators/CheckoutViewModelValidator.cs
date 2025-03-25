using ETicaretData.ViewModels;
using FluentValidation;

namespace ETicaretData.Validators
{
    public class CheckoutViewModelValidator : AbstractValidator<CheckoutViewModel>
    {
        public CheckoutViewModelValidator()
        {
            RuleFor(x => x.FirstName)
                .NotEmpty().WithMessage("Ad alanı boş bırakılamaz.")
                .Length(2, 50).WithMessage("Ad 2-50 karakter arasında olmalıdır.");

            RuleFor(x => x.Surname)
                .NotEmpty().WithMessage("Soyad alanı boş bırakılamaz.")
                .Length(2, 50).WithMessage("Soyad 2-50 karakter arasında olmalıdır.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("E-posta adresi boş bırakılamaz.")
                .EmailAddress().WithMessage("Geçerli bir e-posta adresi giriniz.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Telefon numarası boş bırakılamaz.")
                .Matches(@"^[0-9]{10}$").WithMessage("Geçerli bir telefon numarası giriniz.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Adres alanı boş bırakılamaz.")
                .MaximumLength(500).WithMessage("Adres en fazla 500 karakter olabilir.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("Şehir alanı boş bırakılamaz.")
                .Length(2, 50).WithMessage("Şehir 2-50 karakter arasında olmalıdır.");

            RuleFor(x => x.PostalCode)
                .NotEmpty().WithMessage("Posta kodu boş bırakılamaz.")
                .Matches(@"^[0-9]{5}$").WithMessage("Geçerli bir posta kodu giriniz.");

            RuleFor(x => x.CardNumber)
                .NotEmpty().WithMessage("Kart numarası boş bırakılamaz.")
                .Matches(@"^[0-9]{16}$").WithMessage("Geçerli bir kart numarası giriniz.");

            RuleFor(x => x.CardHolderName)
                .NotEmpty().WithMessage("Kart üzerindeki isim boş bırakılamaz.")
                .Length(2, 100).WithMessage("Kart üzerindeki isim 2-100 karakter arasında olmalıdır.");

            RuleFor(x => x.ExpiryMonth)
                .NotEmpty().WithMessage("Son kullanma ayı boş bırakılamaz.")
                .InclusiveBetween(1, 12).WithMessage("Geçerli bir ay giriniz.");

            RuleFor(x => x.ExpiryYear)
                .NotEmpty().WithMessage("Son kullanma yılı boş bırakılamaz.")
                .GreaterThanOrEqualTo(DateTime.Now.Year).WithMessage("Geçerli bir yıl giriniz.");

            RuleFor(x => x.Cvv)
                .NotEmpty().WithMessage("CVV kodu boş bırakılamaz.")
                .Matches(@"^[0-9]{3,4}$").WithMessage("Geçerli bir CVV kodu giriniz.");
        }
    }
} 