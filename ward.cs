using System;
using System.Collections.Generic;
using System.Linq;

class Ward
{
    public int Number { get; }
    public int Capacity { get; }
    public List<Patient> Patients { get; } = new();

    public Ward(int number, int capacity)
    {
        Number = number;
        Capacity = capacity;
    }

    public bool HasSpace => Patients.Count < Capacity;

    public bool TryAdmit(Patient patient)
    {
        if (!HasSpace) return false;
        Patients.Add(patient);
        return true;
    }

    public void Print() =>
        Console.WriteLine($"Палата {Number}: {Patients.Count}/{Capacity}" +
                          (Patients.Count > 0 ? $" ({string.Join(", ", Patients.Select(p => p.Name))})" : ""));
}