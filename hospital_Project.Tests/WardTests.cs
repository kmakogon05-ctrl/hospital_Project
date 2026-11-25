public class WardTests
{
    [Fact]
    public void GeneralWard_AcceptsOnlySeverityOneOrTwo()
    {
        var ward = new GeneralWard(1, 2);

        Assert.True(ward.CanAccept(new Patient("А", TestData.Infarct)));    // 1
        Assert.True(ward.CanAccept(new Patient("Б", TestData.Fracture)));   // 2
        Assert.False(ward.CanAccept(new Patient("В", TestData.Flu)));       // 4
    }

    [Fact]
    public void IntensiveWard_AcceptsOnlySeverityOne()
    {
        var ward = new IntensiveWard(1, 1);

        Assert.True(ward.CanAccept(new Patient("А", TestData.Infarct)));
        Assert.False(ward.CanAccept(new Patient("Б", TestData.Fracture)));
    }

    [Fact]
    public void Constructor_CapacityBelowOne_Throws()
    {
        Assert.Throws<ArgumentException>(() => new GeneralWard(1, 0));
    }

    [Fact]
    public void TryAdmit_FullWard_ReturnsFalse()
    {
        var ward = new IntensiveWard(1, 1);
        Assert.True(ward.TryAdmit(new Patient("А", TestData.Infarct)));

        bool second = ward.TryAdmit(new Patient("Б", TestData.Appendicitis));

        Assert.False(second);
        Assert.False(ward.HasSpace);
        Assert.Single(ward.Patients);
    }

    [Fact]
    public void TryAdmit_WrongSeverity_ReturnsFalse()
    {
        var ward = new IntensiveWard(1, 2);

        Assert.False(ward.TryAdmit(new Patient("А", TestData.Fracture)));
        Assert.Empty(ward.Patients);
    }

    [Fact]
    public void Discharge_ByName_IsCaseInsensitive_AndFreesSpace()
    {
        var ward = new GeneralWard(1, 1);
        ward.TryAdmit(new Patient("Олег", TestData.Pneumonia));

        Patient? discharged = ward.Discharge("олег");

        Assert.NotNull(discharged);
        Assert.True(ward.HasSpace);
    }

    [Fact]
    public void Discharge_UnknownName_ReturnsNull()
    {
        var ward = new GeneralWard(1, 1);

        Assert.Null(ward.Discharge("Ніхто"));
    }

    [Fact]
    public void Discharge_ByPatient_RemovesHim()
    {
        var ward = new GeneralWard(1, 1);
        var patient = new Patient("Олег", TestData.Pneumonia);
        ward.TryAdmit(patient);

        Assert.True(ward.Discharge(patient));
        Assert.False(ward.Discharge(patient));   // вдруге вже нікого видаляти
    }

    [Fact]
    public void Describe_EmptyWard_SaysFree()
    {
        var ward = new GeneralWard(3, 2);

        Assert.Equal("Палата 3 [звичайна] 0/2: вільна", ward.Describe());
    }

    [Fact]
    public void Describe_ShowsPatients()
    {
        var ward = new GeneralWard(3, 2);
        ward.TryAdmit(new Patient("Олег", TestData.Pneumonia));

        Assert.Equal("Палата 3 [звичайна] 1/2: Олег (Пневмонія, пріоритет 2)", ward.Describe());
    }
}
