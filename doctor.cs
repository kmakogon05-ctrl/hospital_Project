using System;
using System.Collections.Generic;

// Абстрактний лікар. Конкретні лікарі (Therapist, Surgeon, Cardiologist) успадковують його.
abstract class Doctor : Person
{
    private readonly List<string> treated = new();     // приховано: змінювати можна лише через Treat

    public Specialty Specialty { get; }
    public IReadOnlyList<string> Treated => treated;   // назовні лише для читання

    protected Doctor(string name, Specialty specialty) : base(name)
    {
        Specialty = specialty;
    }

    public override string Role => "Лікар (" + Specialty.ToUkrainian() + ")";

    // Що саме робить лікар при прийомі: у кожного нащадка своє
    protected abstract string Action { get; }

    public bool CanTreat(Disease disease) => disease.RequiredSpecialty == Specialty;

    // virtual: нащадки можуть доповнити поведінку
    public virtual void Treat(Patient patient)
    {
        treated.Add(patient.Name);
        Console.WriteLine($"  [{Role} {Name}] {Action}: {patient.Name} ({patient.Disease.Name})");
    }

    public override string Describe() =>
        $"{base.Describe()}, прийнято пацієнтів: {treated.Count}";
}
