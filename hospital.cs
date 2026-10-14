using System;
using System.Collections.Generic;
using System.Linq;

class Hospital
{
    private readonly EmergencyRoom emergency = new();
    private readonly Queue<Doctor> doctors = new();
    private readonly Dictionary<int, Ward> wards = new();
    private readonly HashSet<string> registered = new();
    private readonly List<string> discharged = new();
    private readonly List<string> noBed = new();

    public void AddDoctor(Doctor doctor) => doctors.Enqueue(doctor);
    public void AddWard(Ward ward) => wards[ward.Number] = ward;

    public void Register(Patient patient)
    {
        if (patient.Severity < 1 ||patient.Severity > 5)
        {
            Console.WriteLine($"{patient.Name}: некоректний пріоритет");
            return;
        }
        if (!registered.Add(patient.Name))
        {
            Console.WriteLine($"{patient.Name}: уже зареєстрований");
            return;
        }
        emergency.Register(patient);
    }

    public void ProcessNext()
    {
        var patient = emergency.TakeNext();
        if (patient == null || doctors.Count == 0) return;

        var doctor = doctors.Dequeue();
        doctor.Treat(patient);
        doctors.Enqueue(doctor);

        if (patient.Severity <= 2)
        {
            var ward = wards.Values.FirstOrDefault(w => w.HasSpace);
            if (ward != null && ward.TryAdmit(patient))
                Console.WriteLine($"  -> госпіталізовано в палату {ward.Number}");
            else
            {
                noBed.Add(patient.Name);
                Console.WriteLine("  -> немає вільних місць у палатах");
            }
        }
        else
        {
            discharged.Add(patient.Name);
            Console.WriteLine("  -> відправлено додому");
        }
    }

    public void ProcessAll()
    {
        while (emergency.Waiting > 0)
            ProcessNext();
    }

    public void PrintReport()
    {
        Console.WriteLine("\n=== Звіт ===");
        foreach (var w in wards.Values) w.Print();
        foreach (var d in doctors)
            Console.WriteLine($"{d.Name}: прийнято {d.Treated.Count}");
        Console.WriteLine("Виписано: " + string.Join(", ", discharged));
        Console.WriteLine("Без місця в палаті: " + string.Join(", ", noBed));
    }
}