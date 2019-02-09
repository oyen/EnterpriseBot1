namespace EnterpriseBot1.Dialogs.Shared
{
    public class UserProfile
    {
        public UserProfile(string name, string email, string number)
        {
            this.UserName = name;
            this.UserEmail = email;
            this.PhoneNumber = number;
        }

        public string UserName { get; set; }

        public string UserEmail { get; set; }

        public string PhoneNumber { get; set; }
    }
}
