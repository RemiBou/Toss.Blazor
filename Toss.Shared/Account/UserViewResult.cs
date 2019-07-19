namespace Toss.Shared.Account
{
    public class UserViewResult
    {
        public UserViewResult()
        {
        }

        public UserViewResult(string userName, string bio)
        {
            UserName = userName;
            Bio = bio;
        }

        public string UserName { get; set; }

        public string Bio { get; set; }
    }

}