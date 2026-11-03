using System;

// Пацієнт успадковує Person (отримує Name) і реалізує IComparable:
// пацієнтів можна порівнювати й сортувати за терміновістю.
class Patient : Person, IComparable<Patient>
{
    private static int nextId = 1;      // static: лічильник спільний для всіх пацієнтів

    public int Id { get; }              // порядковий номер реєстрації
    public Disease Disease { get; }
    public int Severity => Disease.Severity;   // терміновість береться з хвороби
    public Doctor? AttendingDoctor { get; private set; }   // змінити можна лише всередині класу

    public Patient(string name, Disease disease) : base(name)
    {
        Disease = disease ?? throw new ArgumentNullException(nameof(disease));
        Id = nextId++;
    }

    public override string Role => "Пацієнт";

    // Перевизначення віртуального методу батька
    public override string Describe() =>
        $"{Role} {Name}, хвороба: {Disease.Name}, пріоритет {Severity}";

    public void AssignDoctor(Doctor doctor) => AttendingDoctor = doctor;

    // Спочатку терміновіший (менше число), при рівності той, хто прийшов раніше
    public int CompareTo(Patient? other)
    {
        if (other == null) return 1;
        int bySeverity = Severity.CompareTo(other.Severity);
        return bySeverity != 0 ? bySeverity : Id.CompareTo(other.Id);
    }
}
