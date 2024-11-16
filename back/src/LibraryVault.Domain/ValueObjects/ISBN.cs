namespace LibraryVault.Domain.ValueObjects
{
    public class ISBN
    {
        public string Value { get; private set; }

        public ISBN(string value)
        {
            if (string.IsNullOrWhiteSpace(value) || value.Length != 13)
                throw new ArgumentException("Invalid ISBN.");

            Value = value;
        }

        public override bool Equals(object obj)
        {
            return obj is ISBN isbn && Value == isbn.Value;
        }

        public override int GetHashCode() => Value.GetHashCode();
    }
}