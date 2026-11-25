public class HospitalTests
{
    // ---------- Прийом пацієнтів ----------

    [Fact]
    public void Admit_MildDisease_IsOutpatient_AndGoesToDischargedList()
    {
        Hospital hospital = TestData.SmallHospital();

        AdmissionResult result = hospital.Admit("Тарас", TestData.Flu);

        Assert.Equal(AdmissionStatus.Outpatient, result.Status);
        Assert.True(result.CanBeAdmittedNow);
        Assert.Contains("Виписано: Тарас", hospital.BuildReport());
    }

    [Fact]
    public void Admit_SevereDisease_GoesToFreeWard()
    {
        Hospital hospital = TestData.SmallHospital();

        AdmissionResult result = hospital.Admit("Марія", TestData.Infarct);

        Assert.Equal(AdmissionStatus.Admitted, result.Status);
        Assert.Contains("Палата 1 [реанімація] 1/1: Марія", hospital.BuildReport());
    }

    [Fact]
    public void Admit_MoreUrgentPatient_DisplacesLessUrgentOneToQueue()
    {
        Hospital hospital = TestData.SmallHospital();
        hospital.Admit("Марія", TestData.Infarct);       // реанімація
        hospital.Admit("Олег", TestData.Pneumonia);      // звичайна палата

        AdmissionResult result = hospital.Admit("Андрій", TestData.Appendicitis);

        Assert.Equal(AdmissionStatus.AdmittedWithDisplacement, result.Status);
        string report = hospital.BuildReport();
        Assert.Contains("Палата 2 [звичайна] 1/1: Андрій", report);
        Assert.Contains("Пацієнт Олег", report);          // тепер у черзі
        Assert.Contains("Черга очікування (1):", report);
    }

    [Fact]
    public void Admit_NoSpaceAndNobodyToDisplace_GoesToWaiting()
    {
        Hospital hospital = TestData.SmallHospital();
        hospital.Admit("Марія", TestData.Infarct);
        hospital.Admit("Андрій", TestData.Appendicitis);   // обидва пріоритету 1, палати зайняті

        AdmissionResult result = hospital.Admit("Богдан", TestData.Appendicitis);

        Assert.Equal(AdmissionStatus.Waiting, result.Status);
        Assert.False(result.CanBeAdmittedNow);
        Assert.Contains("Черга очікування (1):", hospital.BuildReport());
    }

    [Fact]
    public void Admit_SamePatientTwice_IsRejected()
    {
        Hospital hospital = TestData.SmallHospital();
        hospital.Admit("Марія", TestData.Infarct);

        AdmissionResult second = hospital.Admit("марія", TestData.Infarct);

        Assert.Equal(AdmissionStatus.Rejected, second.Status);
        Assert.False(second.CanBeAdmittedNow);
    }

    [Fact]
    public void Admit_NoDoctorOfRequiredSpecialty_IsRejected()
    {
        var hospital = new Hospital();
        hospital.AddDoctor(new Therapist("Петренко"));
        hospital.AddWard(new IntensiveWard(1, 1));

        AdmissionResult result = hospital.Admit("Марія", TestData.Infarct);   // потрібен кардіолог

        Assert.Equal(AdmissionStatus.Rejected, result.Status);
    }

    [Fact]
    public void Admit_EmptyName_IsRejected_WithoutThrowing()
    {
        Hospital hospital = TestData.SmallHospital();

        AdmissionResult result = hospital.Admit("   ", TestData.Flu);

        Assert.Equal(AdmissionStatus.Rejected, result.Status);
    }

    // ---------- Виписка ----------

    [Fact]
    public void Discharge_MovesMostUrgentPatientFromQueueIntoFreedBed()
    {
        Hospital hospital = TestData.SmallHospital();
        hospital.Admit("Марія", TestData.Infarct);
        hospital.Admit("Андрій", TestData.Appendicitis);
        hospital.Admit("Богдан", TestData.Appendicitis);   // у черзі

        bool result = hospital.Discharge("Марія");

        Assert.True(result);
        string report = hospital.BuildReport();
        Assert.Contains("Палата 1 [реанімація] 1/1: Богдан", report);
        Assert.Contains("Черга очікування порожня.", report);
        Assert.Contains("Виписано: Марія", report);
    }

    [Fact]
    public void Discharge_UnknownPatient_ReturnsFalse()
    {
        Hospital hospital = TestData.SmallHospital();

        Assert.False(hospital.Discharge("Ніхто"));
    }

    [Fact]
    public void Discharge_PatientWhoIsOnlyInQueue_ReturnsFalse()
    {
        Hospital hospital = TestData.SmallHospital();
        hospital.Admit("Марія", TestData.Infarct);
        hospital.Admit("Андрій", TestData.Appendicitis);
        hospital.Admit("Богдан", TestData.Appendicitis);   // у черзі

        Assert.False(hospital.Discharge("Богдан"));
    }

    // ---------- Звіт у файл ----------

    [Fact]
    public void BuildReport_ContainsAllSections()
    {
        Hospital hospital = TestData.SmallHospital();

        string report = hospital.BuildReport();

        Assert.Contains("=== Звіт ===", report);
        Assert.Contains("Палати:", report);
        Assert.Contains("Лікарі:", report);
        Assert.Contains("Виписано: ніхто", report);
    }

    [Fact]
    public void SaveReport_WritesReportToFile()
    {
        string dir = Path.Combine(Path.GetTempPath(), "hospital_tests_" + Guid.NewGuid());
        string path = Path.Combine(dir, "report.txt");   // папки ще немає: вона має створитися
        try
        {
            Hospital hospital = TestData.SmallHospital();
            hospital.Admit("Марія", TestData.Infarct);

            bool saved = hospital.SaveReport(path);

            Assert.True(saved);
            Assert.True(File.Exists(path));
            string content = File.ReadAllText(path);
            Assert.Contains("=== Звіт ===", content);
            Assert.Contains("Марія", content);
        }
        finally
        {
            if (Directory.Exists(dir)) Directory.Delete(dir, true);
        }
    }

    [Fact]
    public void SaveReport_InvalidPath_ReturnsFalse_AndLogsError()
    {
        var logger = new ListLogger();
        Hospital hospital = TestData.SmallHospital(logger);

        bool saved = hospital.SaveReport("bad\0path.txt");   // символ \0 у шляху недопустимий

        Assert.False(saved);
        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Error);
    }

    // ---------- Логування ----------

    [Fact]
    public void Admit_LogsInfo_ForAcceptedPatient()
    {
        var logger = new ListLogger();
        Hospital hospital = TestData.SmallHospital(logger);

        hospital.Admit("Марія", TestData.Infarct);

        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Info && e.Message.Contains("Марія"));
    }

    [Fact]
    public void Admit_LogsWarning_ForRejectedPatient()
    {
        var logger = new ListLogger();
        Hospital hospital = TestData.SmallHospital(logger);
        hospital.Admit("Марія", TestData.Infarct);

        hospital.Admit("Марія", TestData.Infarct);   // дубль

        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Warning && e.Message.Contains("Rejected"));
    }

    [Fact]
    public void Discharge_LogsInfo()
    {
        var logger = new ListLogger();
        Hospital hospital = TestData.SmallHospital(logger);
        hospital.Admit("Марія", TestData.Infarct);

        hospital.Discharge("Марія");

        Assert.Contains(logger.Entries, e => e.Level == LogLevel.Info && e.Message.Contains("Виписано"));
    }

    [Fact]
    public void Hospital_WorksWithoutLogger()
    {
        Hospital hospital = TestData.SmallHospital();   // logger = null -> NullLogger

        var exception = Record.Exception(() => hospital.Admit("Марія", TestData.Infarct));

        Assert.Null(exception);
    }
}
