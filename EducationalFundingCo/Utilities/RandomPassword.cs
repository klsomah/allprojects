using System.Collections.Generic;
using System.Text;
using System;
using System.Linq;

namespace EducationalFundingCo.Utilities
{
    public class RandomPassword
    {
        public List<char> chars = new List<char>();

        //public string GeneratePassword()
        //{
        //    AddChars(ref chars);
        //    StringBuilder sb = new StringBuilder();
        //    Random random = new Random();
        //    int j = 0;
        //    while (j < 10)
        //    {
        //        sb.Append(chars[random.Next(0, chars.Count)]);
        //        j++;
        //    }
        //    return sb.ToString();
        //    }
        //public string GeneratePassword(int length = 10)
        //{
        //    AddChars(ref chars);
        //    string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        //    string lowercase = "abcdefghijklmnopqrstuvwxyz";
        //    string numbers = "0123456789";
        //    string specialChars = "!@#$+";
        //    string allChars = uppercase + lowercase + numbers + specialChars;
        //    StringBuilder sb = new StringBuilder();
        //    Random random = new Random();
        //    string password = new string(
        //        Enumerable.Repeat(allChars, length)
        //                  .Select(s => s[random.Next(s.Length)])
        //                  .ToArray()
        //    );
        //    return password.ToString();
        //}
        //    public void AddChars(ref List<char> chars)
        //{
        //    for (char c = 'a'; c <= 'z'; c++)
        //    {
        //        chars.Add(c);
        //    }
        //    for (char c = 'A'; c <= 'Z'; c++)
        //    {
        //        chars.Add(c);
        //    }
        //    for (char c = '!'; c <= '?'; c++)
        //    {
        //        chars.Add(c);
        //    }
        //    for (char c = '0'; c <= '9'; c++)
        //    {
        //        chars.Add(c);
        //    }
        //}

        public string GenerateRandomPassword()
        {
            string uppercaseLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            string lowercaseLetters = "abcdefghijklmnopqrstuvwxyz";
            string digits = "0123456789";
            string specialCharacters = "!@#$%^&*()_+-=[]{}|;':\"<>,.?/";

            Random random = new Random();

            // Randomly select one character from each category
            char uppercase = uppercaseLetters[random.Next(uppercaseLetters.Length)];
            char lowercase = lowercaseLetters[random.Next(lowercaseLetters.Length)];
            char alphanumeric = uppercaseLetters[random.Next(uppercaseLetters.Length)];
            char digit = digits[random.Next(digits.Length)];
            char specialCharacter = specialCharacters[random.Next(specialCharacters.Length)];

            // Combine the selected characters with random characters to form the password
            string randomChars = uppercaseLetters + lowercaseLetters + digits + specialCharacters;
            char[] passwordChars = new char[10];
            passwordChars[0] = uppercase;
            passwordChars[1] = lowercase;
            passwordChars[2] = alphanumeric;
            passwordChars[3] = digit;
            passwordChars[4] = specialCharacter;

            for (int i = 5; i < passwordChars.Length; i++)
            {
                passwordChars[i] = randomChars[random.Next(randomChars.Length)];
            }

            // Shuffle the characters to randomize the password
            passwordChars = passwordChars.OrderBy(c => random.Next()).ToArray();

            return new string(passwordChars);
        }

    }
}
