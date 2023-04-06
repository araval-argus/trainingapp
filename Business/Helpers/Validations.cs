using ChatApp.Models;
using System.Text.RegularExpressions;

namespace ChatApp.Business.Helpers
{
    public  class Validations
    {
        

        public bool ValidateRegistrationField(RegisterModel registerModel)
        {
           

            // check email pattern and password length
            if(!IsValidEmail(registerModel.Email) || (registerModel.Password.Length > 16 && registerModel.Password.Length < 8))
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


        
    }
}
