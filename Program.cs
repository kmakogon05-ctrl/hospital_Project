using System;

class Program
{
    static void Main()
    {
        var hospital = new Hospital();
        hospital.AddDoctor(new Doctor("Петренко", "терапевт"));
        hospital.AddDoctor(new Doctor("Коваль", "хірург"));
        hospital.AddWard(new Ward(1, 1));
        hospital.AddWard(new Ward(2, 1));

        hospital.Register(new Patient("Олег", 3));
        hospital.Register(new Patient("Марія", 1));
        hospital.Register(new Patient("Денис", 2));
        hospital.Register(new Patient("Богдан", 1));
        hospital.Register(new Patient("Олег", 4)); // дублікат
        hospital.Register(new Patient("Софія", 5));

        Console.WriteLine();
        hospital.ProcessAll();
        hospital.PrintReport();
    }
}