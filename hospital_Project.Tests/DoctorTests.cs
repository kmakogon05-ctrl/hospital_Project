public class DoctorTests
{
    [Fact]
    public void CanTreat_MatchesSpecialty()
    {
        var surgeon = new Surgeon("Коваль");

        Assert.True(surgeon.CanTreat(TestData.Fracture));
        Assert.False(surgeon.CanTreat(TestData.Infarct));
    }

    [Fact]
    public void Treat_AddsPatientToTreatedList()
    {
        var therapist = new Therapist("Петренко");
        var patient = new Patient("Олег", TestData.Pneumonia);

        therapist.Treat(patient);

        Assert.Contains("Олег", therapist.Treated);
    }

    [Fact]
    public void Describe_ContainsRoleNameAndCount()
    {
        var cardiologist = new Cardiologist("Шевченко");

        string text = cardiologist.Describe();

        Assert.Contains("кардіолог", text);
        Assert.Contains("Шевченко", text);
        Assert.Contains("прийнято пацієнтів: 0", text);
    }

    [Fact]
    public void Doctor_EmptyName_Throws()
    {
        Assert.Throws<ArgumentException>(() => new Therapist(" "));
    }
}
