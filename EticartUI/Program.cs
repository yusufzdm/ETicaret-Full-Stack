using ETicaretDal.Abstract;
using ETicaretDal.Concreate;
using ETicaretData.Context;
using ETicaretData.Entities;
using ETicaretData.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using FluentValidation;
using FluentValidation.AspNetCore;
using ETicaretData.Validators;
using ETicaretData.ViewModels;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddFluentValidation(fv => 
    {
        fv.RegisterValidatorsFromAssemblyContaining<RegisterViewModelValidator>();
        fv.AutomaticValidationEnabled = true;
        fv.ImplicitlyValidateChildProperties = true;
    });

// Validatör servislerini ekle
builder.Services.AddScoped<IValidator<RegisterViewModel>, RegisterViewModelValidator>();
builder.Services.AddScoped<IValidator<LoginViewModel>, LoginViewModelValidator>();
builder.Services.AddScoped<IValidator<Product>, ProductValidator>();
builder.Services.AddScoped<IValidator<CheckoutViewModel>, CheckoutViewModelValidator>();
builder.Services.AddScoped<IValidator<ShippingDetails>, ShippingDetailsValidator>();

//Bağımlılık Enjeksiyonu 
builder.Services.AddDbContext<ETicaretContext>();

builder.Services.AddScoped<ICategoryDal, CategoryDal>();
builder.Services.AddScoped<IProductDal, ProductDal>();
builder.Services.AddScoped<IOrderDal, OrderDal>();
builder.Services.AddScoped<IOrderLineDal, OrderLineDal>();

//Identity Kimlik doğrulama
builder.Services.AddIdentity<AppUser, AppRole>(opistion =>
{
    opistion.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);//Kilitleme süresi
    opistion.Lockout.MaxFailedAccessAttempts = 5;//max başarısız giriş
    opistion.Password.RequireDigit = false;//şifrede rakam gerekliliği kaldırılır
    opistion.Password.RequireNonAlphanumeric = false;//şifrelerde özel karekter gerekliliğini kaldırır...
    opistion.Password.RequireLowercase = false;//şifrede küçük harf gerekliliğini kaldırır
    opistion.Password.RequireUppercase = false;//şifrede büyük harf gerekliliğini kaldırıyor 

}).AddEntityFrameworkStores<ETicaretContext>() //Ef ile veri tabanı bağlantısını sağlar
.AddDefaultTokenProviders();//varsayılan token sağlayıcı ekler

builder.Services.ConfigureApplicationCookie(opistion =>
{
    opistion.LoginPath = "/Account/Login";//giriş yapılmadığında yönlendirilecek sayafa 
    opistion.AccessDeniedPath = "/";//yetkisiz erişim olduğunda yönlendirilecek sayfa
    opistion.Cookie = new CookieBuilder
    {
        Name = "AspNetCoreIdentityExampleCookie", //çerez ismi
        HttpOnly=false,//çerez http 
        SameSite=SameSiteMode.Lax,//çerez aynı sitede yapılacak isteklerde geçerli
        SecurePolicy=CookieSecurePolicy.Always//çerez yanlızca https üzerinde iletilecek
    };
    opistion.SlidingExpiration = true;//çerez geçerlilik süresi doldukça yenilenir
    opistion.ExpireTimeSpan = TimeSpan.FromMinutes(15);//çerez geçerlik süresi
});

//oturum
builder.Services.AddSession();//oturum yönetim servisi

var app = builder.Build();//hata verirse yerini değiştir..........

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();//http den https yönlendirilir
app.UseStaticFiles();//

app.UseRouting();//

app.UseAuthentication();//kimlik doğrulama
app.UseAuthorization();//yetkilendirme işlemleri
app.UseSession();//oturum  yönetimi aktif 

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
