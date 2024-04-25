using Microsoft.AspNetCore.Identity;

namespace BookDiariesWeb.Localizations
{
    public class LocalizationIdentityErrorDescriber : IdentityErrorDescriber
    {

        public override IdentityError DuplicateUserName(string userName)
        {
            return new() { Code = "DublicateUserName", Description = $"{userName} adi bashqa biri terefinden istifade edilmishdir" };
            //return base.DuplicateUserName(userName);
        }

        public override IdentityError DuplicateEmail(string email)
        {
            return new() { Code = "DuplicateEmail", Description = $"{email} adli mail adresi bashqa biri terefinden istifade edilmishdir" };
        }

        public override IdentityError PasswordTooShort(int length)
        {
            return new() { Code = "PasswordTooShort", Description = "Shifre en az 6 ishareden ibaret olmalidir" };

        }
    }
}
