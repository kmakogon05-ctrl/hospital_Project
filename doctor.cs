using System;
using System.Collections.Generic;

class Doctor : Person
{
    public string Specialty { get; }
    public List<string> Treated { get; } = new();

    public Doctor(string name, string specialty) : base(name)
    {
        Specialty = specialty;
    }

    public void Treat(Patient patient)
    {
        Treated.Add(patient.Name);
        Console.WriteLine($"Лікар {Name} ({Specialty}): прийом {patient.Name}, пріоритет {patient.Severity}");
    }
}