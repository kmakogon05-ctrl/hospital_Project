public class PatientTests
{
    [Fact]
    public void Constructor_TrimsName_AndTakesSeverityFromDisease()
    {
        var patient = new Patient("  Олег  ", TestData.Pneumonia);

        Assert.Equal("Олег", patient.Name);
        Assert.Equal(TestData.Pneumonia.Severity, patient.Severity);
    }

    [Fact]
    public void Constructor_EmptyName_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Patient("   ", TestData.Flu));
    }

    [Fact]
    public void Constructor_NullDisease_Throws()
    {
        Assert.Throws<ArgumentNullException>(() => new Patient("Олег", null!));
    }

    [Fact]
    public void Ids_AreUniqueAndIncreasing()
    {
        var first = new Patient("А", TestData.Flu);
        var second = new Patient("Б", TestData.Flu);

        Assert.True(second.Id > first.Id);
    }

    [Fact]
    public void CompareTo_MoreUrgentGoesFirst()
    {
        var urgent = new Patient("Терміновий", TestData.Infarct);
        var calm = new Patient("Спокійний", TestData.Flu);

        Assert.True(urgent.CompareTo(calm) < 0);
        Assert.True(calm.CompareTo(urgent) > 0);
    }

    [Fact]
    public void CompareTo_SameSeverity_EarlierArrivalGoesFirst()
    {
        var earlier = new Patient("Раніше", TestData.Pneumonia);
        var later = new Patient("Пізніше", TestData.Pneumonia);

        Assert.True(earlier.CompareTo(later) < 0);
    }

    [Fact]
    public void CompareTo_Null_ReturnsPositive()
    {
        var patient = new Patient("Олег", TestData.Flu);

        Assert.True(patient.CompareTo(null) > 0);
    }

    [Fact]
    public void AssignDoctor_SetsAttendingDoctor()
    {
        var patient = new Patient("Олег", TestData.Pneumonia);
        var doctor = new Therapist("Петренко");

        patient.AssignDoctor(doctor);

        Assert.Same(doctor, patient.AttendingDoctor);
    }
}
