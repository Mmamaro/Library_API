using System.ComponentModel.DataAnnotations;

namespace Library_API.Models
{
    public class TwoFaModel
    {
        public string QrCodeUrl { get; set; }
        public string ManualEntryCode { get; set; }
        public string TwoFAKey { get; set; }
    }

    public class TwoFAResetModel
    {
        public string QrCodeUrl { get; set; }
        public string ManualEntryCode { get; set; }
        public string twoFAKey { get; set; }
        public bool isTwoFAVerified { get; set; }
        public bool isFirstSignIn { get; set; }
    }

    public class TwoFALoginModel
    {
        [EmailAddress] public string Email { get; set; }
        public string Code { get; set; }
    }
}
