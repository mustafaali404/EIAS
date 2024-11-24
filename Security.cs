using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;



namespace EIAS
{
    /// <summary>
    /// Author: Timur Maistrenko
    /// <br></br>
    /// Security Class containing security and cryptography-related methods.
    /// </summary>
    public class Security
    {


        private static readonly int saltLength = 16;
        /// <summary>
        /// Generates salted password hash.
        /// </summary>
        /// <param name="password"></param>
        /// <param name="salt"></param>
        /// <returns>
        /// string hashed password+salt
        /// </returns>
        public static string PasswordHash(string password, string salt)
        {
            
            string saltedPassword = password + salt;
            /*
            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(saltedPassword));

            return Encoding.UTF8.GetString(hash);
            */

            using (SHA256 hash = SHA256.Create())
            {
                return String.Concat(hash
                  .ComputeHash(Encoding.UTF8.GetBytes(saltedPassword))
                  .Select(item => item.ToString("x2")));
            }
        }

        /// <summary>
        /// Generates random salt.
        /// </summary>
        /// <returns>
        /// string salt
        /// </returns>
        public static string GenerateSalt()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random rng = new();

            return new string(Enumerable.Range(1, saltLength).Select(_ => chars[rng.Next(chars.Length)]).ToArray());
        }

        /// <summary>
        /// Sanitizes SQL statements by eliminating the effectiveness of ' escape character injections.
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string Sanitize (string str)
        {
            return str.Replace("'", "''");
        }

        /// <summary>
        /// Estimates whether the email is likely to be valid or not.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool IsValidEmail(string email)
        {
            var trimmedEmail = email.Trim();

            if (trimmedEmail.EndsWith("."))
            {
                return false; 
            }
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == trimmedEmail;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Password regex check.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsValidPassword(string password)
        {
            return new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$").IsMatch(password);
        }

        ///TODO
        ///Login
        ///Token Handling and Verification
        ///HTML Sanitation
        ///String Length Enforcement
        ///Utility
    }


}
