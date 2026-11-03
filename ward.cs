using System;
using System.Collections.Generic;
using System.Linq;

// Абстрактна палата. Види палат (GeneralWard, IntensiveWard) відрізняються
// тим, кого саме вони можуть прийняти (метод CanAccept).
abstract class Ward : IPrintable
{
    private readonly List<Patient> patients = new();   // приховано від інших класів

    public int Number { get; }
    public int Capacity { get; }
    public IReadOnlyList<Patient> Patients => patients;   // назовні лише для читання
    public bool HasSpace => patients.Count < Capacity;

    public abstract string Type { get; }

    protected Ward(int number, int capacity)
    {
        if (capacity < 1)
            throw new ArgumentException("Місткість палати має бути не менше 1.");
        Number = number;
        Capacity = capacity;
    }

    // Абстрактний метод: кожен вид палати вирішує по-своєму (поліморфізм)
    public abstract bool CanAccept(Patient patient);

    public bool TryAdmit(Patient patient)
    {
        if (!HasSpace || !CanAccept(patient)) return false;
        patients.Add(patient);
        return true;
    }

    // Перевантаження методів: однакова назва, різні параметри
    public bool Discharge(Patient patient) => patients.Remove(patient);

    public Patient? Discharge(string name)
    {
        Patient? patient = patients.FirstOrDefault(
            p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (patient != null) patients.Remove(patient);
        return patient;
    }

    public void Print()
    {
        string content = patients.Count > 0
            ? string.Join(", ", patients.Select(p => $"{p.Name} ({p.Disease.Name}, пріоритет {p.Severity})"))
            : "вільна";
        Console.WriteLine($"Палата {Number} [{Type}] {patients.Count}/{Capacity}: {content}");
    }
}
