using System.Security.Cryptography;

namespace Student_Science_Research_Management_UEF.Utilities
{
    public static class PasswordGenerator
    {
        private const string Upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private const string Lower = "abcdefghijklmnopqrstuvwxyz";
        private const string Digits = "0123456789";
        private const string Special = "!@$?_-";

        public static string Generate(int length = 10)
        {
            string all = Upper + Lower + Digits + Special;
            byte[] bytes = new byte[length];

            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(bytes);
            }

            char[] password = new char[length];

            for (int i = 0; i < length; i++)
            {
                password[i] = all[bytes[i] % all.Length];
            }

            return new string(password);
        }
    }
}
