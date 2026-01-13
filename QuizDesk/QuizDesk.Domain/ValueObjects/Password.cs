namespace QuizDesk.Domain.ValueObjects
{
    public class Password : ValueObject
    {
        public string Hash { get; }

        private Password(string hash)
        {
             if (string.IsNullOrWhiteSpace(hash))
                throw new ArgumentException("Password hash cannot be empty.", nameof(hash));

            Hash = hash;
        }

        public static Password Create(string hash)
        {
            return new Password(hash);
        }

        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Hash;
        }
    }
}
