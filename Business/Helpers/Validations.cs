using ChatApp.Models;
using System.Text.RegularExpressions;

namespace ChatApp.Business.Helpers
{
    public  class Validations
    {
        public bool ValidateLoginField(LoginModel loginModel)
        {
            if ((IsFieldNullOrEmpty(loginModel.Username) || !IsValidEmail(loginModel.EmailAddress)) || IsFieldNullOrEmpty(loginModel.Password))
            {
                return false;
            }
            return true;
        }

        public bool ValidateRegistrationField(RegisterModel registerModel)
        {
            // checking any field is empty or null
            if(IsFieldNullOrEmpty(registerModel.FirstName) ||
               IsFieldNullOrEmpty(registerModel.LastName)  ||
               IsFieldNullOrEmpty(registerModel.UserName)  ||
               IsFieldNullOrEmpty(registerModel.Password))
            {
                return false;
            }

            // check email pattern and password length
            if(!IsValidEmail(registerModel.Email) || registerModel.Password.Length < 6)
            {
                return false;
            }

            return true;
        }

        //validate email address
        private bool IsValidEmail(string email)
        {
            string emailRegEx = @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z";

            Regex re = new Regex(emailRegEx);

            return re.IsMatch(email);
        }


        private bool IsFieldNullOrEmpty(string field)
        {
            return field.Equals(null) && field.Equals("");
        }
    }
}
