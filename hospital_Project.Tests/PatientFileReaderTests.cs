public class PatientFileReaderTests
{
    [Fact]
    public void Parse_ValidLines_ReturnsEntries()
    {
        var import = PatientFileReader.Parse(new[] { "Олег;Пневмонія", "Марія;Інфаркт міокарда" });

        Assert.Equal(2, import.Entries.Count);
        Assert.Empty(import.Errors);
        Assert.Equal("Олег", import.Entries[0].Name);
        Assert.Equal("Пневмонія", import.Entries[0].Disease.Name);
    }

    [Fact]
    public void Parse_SkipsEmptyLinesAndComments()
    {
        var import = PatientFileReader.Parse(new[] { "# коментар", "", "   ", "Олег;Грип" });

        Assert.Single(import.Entries);
        Assert.Empty(import.Errors);
    }

    [Fact]
    public void Parse_TrimsSpacesAroundFields()
    {
        var import = PatientFileReader.Parse(new[] { "  Олег  ;  грип  " });

        Assert.Equal("Олег", import.Entries[0].Name);
        Assert.Equal("Грип", import.Entries[0].Disease.Name);
    }

    [Theory]
    [InlineData("Олег")]                    // немає розділювача
    [InlineData("Олег;Грип;зайве")]         // забагато полів
    [InlineData(";Грип")]                   // порожнє ім'я
    [InlineData("Олег;")]                   // порожня хвороба
    public void Parse_WrongFormat_AddsError(string line)
    {
        var import = PatientFileReader.Parse(new[] { line });

        Assert.Empty(import.Entries);
        Assert.Single(import.Errors);
        Assert.Contains("Рядок 1", import.Errors[0]);
    }

    [Fact]
    public void Parse_UnknownDisease_AddsError_AndContinues()
    {
        var import = PatientFileReader.Parse(new[] { "Олег;Невідома хвороба", "Марія;Грип" });

        Assert.Single(import.Entries);
        Assert.Single(import.Errors);
        Assert.Contains("невідома хвороба", import.Errors[0]);
        Assert.Contains("Рядок 1", import.Errors[0]);
    }

    [Fact]
    public void ReadFile_ReadsPatientsFromRealFile()
    {
        string path = Path.Combine(Path.GetTempPath(), "patients_" + Guid.NewGuid() + ".txt");
        try
        {
            File.WriteAllLines(path, new[] { "# тест", "Олег;Пневмонія", "Марія;Аритмія" });

            var import = PatientFileReader.ReadFile(path);

            Assert.Equal(2, import.Entries.Count);
            Assert.Empty(import.Errors);
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Fact]
    public void ReadFile_MissingFile_ThrowsFileNotFound()
    {
        string path = Path.Combine(Path.GetTempPath(), "no_such_" + Guid.NewGuid() + ".txt");

        Assert.Throws<FileNotFoundException>(() => PatientFileReader.ReadFile(path));
    }

    [Fact]
    public void FileFromFile_CanBeAdmittedToHospital()
    {
        // Наскрізна перевірка: файл -> зчитування -> прийом у лікарню
        var import = PatientFileReader.Parse(new[] { "Марія;Інфаркт міокарда", "Тарас;Грип" });
        Hospital hospital = TestData.SmallHospital();

        var results = import.Entries.Select(e => hospital.Admit(e.Name, e.Disease)).ToList();

        Assert.Equal(AdmissionStatus.Admitted, results[0].Status);
        Assert.Equal(AdmissionStatus.Outpatient, results[1].Status);
    }
}
