using Library_API.Models;
using Library_API.Repositories;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Text;
using Google.Authenticator;

namespace Library_API.Services
{
    public class TwoFaService
    {
        private readonly ILogger<TwoFaService> _logger;
        private readonly IAuth _repo;

        public TwoFaService(ILogger<TwoFaService> logger, IAuth repo)
        {
            _logger = logger;
            _repo = repo;
        }

        #region [ TwoFA Authentication ]

        public TwoFaModel TwoFASetup(string Email)
        {
            try
            {

                // Create Secret Key 
                string key = Guid.NewGuid().ToString().Replace("-", "");

                // Remove any white spaces from secret
                var Regkey = Regex.Replace(key, @"\s+", "");

                // Encrypt Secret Key
                byte[] encryptedBytes = EncryptString(Regkey);
                string encryptedText = Convert.ToBase64String(encryptedBytes);

                // Check if Secret already exists in DB. If it exists, create another key
                var RegKeyUnique = _repo.TwoFaKeyExists(encryptedText);

                if (RegKeyUnique != null)
                {
                    key = Guid.NewGuid().ToString().Replace("-", "");
                    Regkey = Regex.Replace(key, @"\s+", "");
                    encryptedBytes = EncryptString(Regkey);
                    encryptedText = Convert.ToBase64String(encryptedBytes);
                }

                // Creating MFA QR code and Manual Code 
                TwoFactorAuthenticator tfa = new TwoFactorAuthenticator();
                SetupCode setupInfo = tfa.GenerateSetupCode("Library Management System", Email, Regkey, false, 3);

                var _2fa = new TwoFaModel()
                {
                    TwoFAKey = encryptedText,
                    QrCodeUrl = setupInfo.QrCodeSetupImageUrl,
                    ManualEntryCode = setupInfo.ManualEntryKey
                };

                _logger.LogInformation($"User: {Email} MFA has been setup.");

                return _2fa;

            }
            catch (Exception ex)
            {
                _logger.LogError($"Could not setup MFA for user {Email} with reason: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region [ Encryption ]

        public static byte[] EncryptString(string EncryptedText)
        {
            byte[] encryptedBytes;
            byte[] salt = Encoding.UTF8.GetBytes("IzhJ03F8nlljKSWamBuIg9raj69f6MpdW4yZ9feR"); // Salt value for additional security
            string password = "Ferrari458@Italia";

            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(password, salt, 1000);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] data = Encoding.UTF8.GetBytes(EncryptedText);
                        cryptoStream.Write(data, 0, data.Length);
                        cryptoStream.FlushFinalBlock();
                    }

                    encryptedBytes = memoryStream.ToArray();
                }
            }

            return encryptedBytes;
        }

        #endregion

        #region [ Decryption ]

        public string DecryptString(byte[] encryptedBytes)
        {
            string decryptedText;
            string password = "Ferrari458@Italia";

            byte[] salt = Encoding.UTF8.GetBytes("IzhJ03F8nlljKSWamBuIg9raj69f6MpdW4yZ9feR"); // Salt value used during encryption

            using (RijndaelManaged aes = new RijndaelManaged())
            {
                aes.KeySize = 256;
                aes.BlockSize = 128;

                var key = new Rfc2898DeriveBytes(password, salt, 1000);
                aes.Key = key.GetBytes(aes.KeySize / 8);
                aes.IV = key.GetBytes(aes.BlockSize / 8);

                using (MemoryStream memoryStream = new MemoryStream(encryptedBytes))
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        byte[] decryptedBytes = new byte[encryptedBytes.Length];
                        int decryptedByteCount = cryptoStream.Read(decryptedBytes, 0, decryptedBytes.Length);

                        decryptedText = Encoding.UTF8.GetString(decryptedBytes, 0, decryptedByteCount);
                    }
                }
            }

            return decryptedText;
        }

        #endregion
    }
}
