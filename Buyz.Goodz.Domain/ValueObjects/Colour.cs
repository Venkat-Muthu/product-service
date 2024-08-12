using Ardalis.GuardClauses;

namespace Buyz.Goodz.Domain.ValueObjects;

[Serializable]
public struct Colour : IEquatable<Colour>
{
    public static readonly Colour Default = new Colour("None");
    public string Name { get; private set; }

    public Colour(string name)
    {
        Name = Guard.Against.NullOrWhiteSpace(name);
    }
    public bool Equals(Colour other)
    {
        return Name == other.Name;
    }

    public override bool Equals(object obj)
    {
        if (obj is Colour colour)
        {
            return Equals(colour);
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }

    public override string ToString()
    {
        return Name;
    }
}
