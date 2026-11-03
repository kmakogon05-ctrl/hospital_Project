using System;
using System.Collections.Generic;
using System.Linq;

// Черга очікування: тут чекають пацієнти, для яких зараз немає місця в палаті.
// Список завжди відсортований: терміновіші першими (див. Patient.CompareTo).
class WaitingRoom : IPrintable
{
    private readonly List<Patient> patients = new();

    public int Count => patients.Count;
    public IReadOnlyList<Patient> Patients => patients;

    public void Add(Patient patient)
    {
        patients.Add(patient);
        patients.Sort();   // використовує IComparable<Patient>
    }

    public bool Remove(Patient patient) => patients.Remove(patient);

    // Найтерміновіший пацієнт із черги, якого може прийняти ця палата
    public Patient? FindFor(Ward ward) => patients.FirstOrDefault(p => ward.CanAccept(p));

    public int PositionOf(Patient patient) => patients.IndexOf(patient) + 1;

    public void Print()
    {
        if (patients.Count == 0)
        {
            Console.WriteLine("Черга очікування порожня.");
            return;
        }
        Console.WriteLine($"Черга очікування ({patients.Count}):");
        for (int i = 0; i < patients.Count; i++)
            Console.WriteLine($"  {i + 1}. {patients[i].Describe()}");
    }
}
