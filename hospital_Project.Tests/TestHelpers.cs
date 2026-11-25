// Допоміжні класи для тестів

// Логер, який просто запам'ятовує повідомлення в списку (щоб перевіряти, що саме залогувалось)
class ListLogger : ILogger
{
    public List<(LogLevel Level, string Message)> Entries { get; } = new();

    public void Log(LogLevel level, string message) => Entries.Add((level, message));
}

static class TestData
{
    public static Disease Infarct => Disease.Find("Інфаркт міокарда")!;     // пріоритет 1, кардіолог
    public static Disease Appendicitis => Disease.Find("Гострий апендицит")!; // пріоритет 1, хірург
    public static Disease Fracture => Disease.Find("Перелом кістки")!;       // пріоритет 2, хірург
    public static Disease Pneumonia => Disease.Find("Пневмонія")!;           // пріоритет 2, терапевт
    public static Disease Flu => Disease.Find("Грип")!;                      // пріоритет 4, терапевт

    // Невелика лікарня: по одному лікарю кожної спеціальності,
    // реанімація на 1 місце і звичайна палата на 1 місце.
    public static Hospital SmallHospital(ILogger? logger = null)
    {
        var hospital = new Hospital(logger);
        hospital.AddDoctor(new Therapist("Петренко"));
        hospital.AddDoctor(new Surgeon("Коваль"));
        hospital.AddDoctor(new Cardiologist("Шевченко"));
        hospital.AddWard(new IntensiveWard(1, 1));
        hospital.AddWard(new GeneralWard(2, 1));
        return hospital;
    }
}
