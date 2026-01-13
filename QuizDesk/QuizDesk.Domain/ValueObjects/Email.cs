using System;

namespace QuizDesk.Domain.ValueObjects
{
    public sealed class Email : ValueObject
    {
        public string Value { get; }

        private Email(string value)
        {
            value = value.Trim();
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty", nameof(value));

            if (!IsValidEmail(value))
                throw new ArgumentException("Invalid email format", nameof(value));

            Value = value.ToLowerInvariant();
        }

        public static Email Create(string email)
        {
            return new Email(email);
        }

        private static bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Value;
        }

        public override string ToString() => Value;

        // Implicit conversion
        public static implicit operator string(Email email) => email.Value;
        public static explicit operator Email(string email) => Create(email);
    }
}
