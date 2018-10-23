using System;
using System.Linq;
using Service.Contract;

namespace Service.Implementation
{
    public class PasswordService : IPasswordService
    {
        private readonly Random _random;
        private readonly int _passwordLength;
        private readonly string _passwordCharacters;

        public PasswordService(int passwordLength, string passwordCharacters)
        {
            if (passwordLength <= 0) throw new ArgumentOutOfRangeException(nameof(passwordLength));

            _random = new Random();
            _passwordLength = passwordLength;
            _passwordCharacters = passwordCharacters ?? throw new ArgumentNullException(nameof(passwordCharacters));
        }

        public string GeneratePassword()
        {
            return new string(Enumerable.Repeat(_passwordCharacters, _passwordLength)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
    }
}