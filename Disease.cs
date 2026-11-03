using System;
using System.Collections.Generic;
using System.Linq;

// Хвороба: назва, терміновість (1 - найтерміновіше, 5 - найлегше)
// і спеціальність лікаря, який її лікує.
class Disease
{
    public string Name { get; }
    public int Severity { get; }
    public Specialty RequiredSpecialty { get; }

    public Disease(string name, int severity, Specialty requiredSpecialty)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Назва хвороби не може бути порожньою.");
        if (severity < 1 || severity > 5)
            throw new ArgumentOutOfRangeException(nameof(severity), "Пріоритет має бути від 1 до 5.");

        Name = name;
        Severity = severity;
        RequiredSpecialty = requiredSpecialty;
    }

    // static: довідник хвороб належить класу, а не окремому об'єкту
    public static readonly IReadOnlyList<Disease> Catalog = new List<Disease>
    {
        new Disease("Інфаркт міокарда", 1, Specialty.Cardiologist),
        new Disease("Гострий апендицит", 1, Specialty.Surgeon),
        new Disease("Перелом кістки", 2, Specialty.Surgeon),
        new Disease("Гіпертонічний криз", 2, Specialty.Cardiologist),
        new Disease("Пневмонія", 2, Specialty.Therapist),
        new Disease("Аритмія", 3, Specialty.Cardiologist),
        new Disease("Ангіна", 4, Specialty.Therapist),
        new Disease("Грип", 4, Specialty.Therapist),
        new Disease("Застуда", 5, Specialty.Therapist),
    };

    public static Disease? Find(string name) =>
        Catalog.FirstOrDefault(d => d.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    public override string ToString() =>
        $"{Name} (пріоритет {Severity}, {RequiredSpecialty.ToUkrainian()})";
}
