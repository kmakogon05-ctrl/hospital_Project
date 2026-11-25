public class DiseaseTests
{
    [Fact]
    public void Constructor_SetsProperties()
    {
        var disease = new Disease("Тест", 3, Specialty.Surgeon);

        Assert.Equal("Тест", disease.Name);
        Assert.Equal(3, disease.Severity);
        Assert.Equal(Specialty.Surgeon, disease.RequiredSpecialty);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Constructor_EmptyName_Throws(string name)
    {
        Assert.Throws<ArgumentException>(() => new Disease(name, 1, Specialty.Therapist));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(6)]
    [InlineData(-1)]
    public void Constructor_SeverityOutOfRange_Throws(int severity)
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new Disease("Тест", severity, Specialty.Therapist));
    }

    [Fact]
    public void Find_IsCaseInsensitive()
    {
        Disease? disease = Disease.Find("грип");

        Assert.NotNull(disease);
        Assert.Equal("Грип", disease!.Name);
    }

    [Fact]
    public void Find_UnknownName_ReturnsNull()
    {
        Assert.Null(Disease.Find("Неіснуюча хвороба"));
    }

    [Fact]
    public void Catalog_AllSeveritiesAreValid()
    {
        Assert.All(Disease.Catalog, d => Assert.InRange(d.Severity, 1, 5));
    }
}
