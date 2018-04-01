namespace Sage.Tests
{
    struct Hash
    {
        public readonly string Name;
        public readonly string Value;

        public Hash(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString() =>
            $"Name ({Name}), Hash ({Value})";
    }
}
