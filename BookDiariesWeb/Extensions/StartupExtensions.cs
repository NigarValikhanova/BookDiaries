using BookDiaries.DataAccess.Data;
using BookDiaries.Models.Models;
using BookDiariesWeb.CustomValidations;
using BookDiariesWeb.Localizations;
using Microsoft.AspNetCore.Identity;

namespace BookDiariesWeb.Extensions
{
    public static class StartupExtensions
    {
        public static void AddIdentityWithExt(this IServiceCollection services)
        {
            services.Configure<DataProtectionTokenProviderOptions>(opt =>
            {
                opt.TokenLifespan = TimeSpan.FromHours(1);
            });


            services.AddIdentity<AppUser, IdentityRole>(options =>
            {
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.AllowedForNewUsers = true; //yeni qeydiyyat userdirse passwordu unuda biler. bir nece yazdiqda bloklamaya bilersiz

                options.User.AllowedUserNameCharacters =
             "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789._"; //olmasını istediyiniz vacib karaterleri yazin

                options.User.RequireUniqueEmail = true;

                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
            }).AddPasswordValidator<PasswordValidator>()
                .AddUserValidator<UserValidator>()
                    .AddErrorDescriber<LocalizationIdentityErrorDescriber>()
                        .AddDefaultTokenProviders()
                            .AddEntityFrameworkStores<AppDbContext>();
        }
    }
}
