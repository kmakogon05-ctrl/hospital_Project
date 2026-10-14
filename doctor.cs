using System;
using System.Collections.Generic;

class Doctor
{
    public string Name { get; }
    public string Specialty { get; }
    public List<string> Treated { get; } = new();

    public Doctor(string name, string specialty)
    {
        Name = name;
        Specialty = specialty;
    }

    public void Treat(Patient patient)
    {
        Treated.Add(patient.Name);
        Console.WriteLine($"Лікар {Name} ({Specialty}): прийом {patient.Name}, пріоритет {patient.Severity}");
    }
}