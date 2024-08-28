using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Security.Claims;

namespace TodoListClient
{
    public class UserValidate
    {
        public string Color { get; set; }
        public string ClientCode { get; set; }
        IHttpContextAccessor _httpContextAccessor;

        public UserValidate(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string ValidateUser()
        {
            //check user roles
            string userroles = _httpContextAccessor.HttpContext?.User.FindFirstValue("extension_userRoles");
            string errormsg = string.Empty;

            if (userroles == null || string.IsNullOrWhiteSpace(userroles))
            {
                errormsg = "User does not belong to any role. Please try logout and login. If the problem still persist, please contact your Administrator";
            }
            else
            {
                string[] uroles = userroles.Split(',');
                if (!uroles.Where(u => u.ToLower() == "basic").Any())
                {
                    errormsg = "User does not belong to 'basic' role. Please try logout and login. If the problem still persist, please contact your Administrator";
                }
            }


            //check tax id
            string taxId = _httpContextAccessor.HttpContext?.User.FindFirstValue("extension_TaxId");
            if (taxId == null || string.IsNullOrWhiteSpace(taxId))
            {
                errormsg = "User does not have tax id. Please contact your Administrator.";
            }

            //check tax id
            ClientCode = _httpContextAccessor.HttpContext?.User.FindFirstValue("extension_ClientCode");
            if (ClientCode == null || string.IsNullOrWhiteSpace(ClientCode))
            {
                errormsg = "User does not have client code. Please contact your Administrator.";
            }
            else
            {
                switch (ClientCode)
                {
                    case "CareOregon":
                        Color = "bg-success text-white";

                        break;
                    case "Encora":
                        Color = "bg-primary text-white";
                        break;
                }
            }
            return errormsg;
        }

        public bool IsAdminUser()
        {
            //check user roles
            string userroles = _httpContextAccessor.HttpContext?.User.FindFirstValue("extension_userRoles");
            string errormsg = string.Empty;

            if (userroles == null || string.IsNullOrWhiteSpace(userroles))
            {
                errormsg = "User does not belong to any role. Please try logout and login. If the problem still persist, please contact your Administrator";       
                return false;
            }
            else
            {
                string[] uroles = userroles.Split(',');
                if (!uroles.Where(u=>u.ToLower().Trim()=="admin").Any())
                {
                    errormsg = "User does not belong to 'basic' role. Please try logout and login. If the problem still persist, please contact your Administrator";
                    return false;
                }
            }
            return true;
        }
    }
}
