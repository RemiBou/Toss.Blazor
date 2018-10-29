namespace Toss.Shared.Account
{
    public class LoginCommandResult
    {
        public bool Need2FA { get; set; }
        public bool IsLockout { get; set; }
        public bool IsSuccess { get; set; }
    }
}