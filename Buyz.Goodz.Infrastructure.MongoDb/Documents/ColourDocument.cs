namespace Buyz.Goodz.Infrastructure.MongoDb.Documents;

public class ColourDocument : IEquatable<ColourDocument>
{
    public string Name { get; set; }

    public bool Equals(ColourDocument other)
    {
        if (other == null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Name.Equals(other.Name);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as ColourDocument);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Name);
    }
}
