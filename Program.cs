using System;
using System.Text;

class Program
{
    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding = Encoding.UTF8;

        Hospital hospital = CreateHospital();
        bool demoDone = false;
        bool running = true;

        Console.WriteLine("=== Лікарня: система прийому пацієнтів ===");

        while (running)
        {
            PrintMenu();
            string? choice = Console.ReadLine();
            if (choice == null) break;          // кінець вводу
            Console.WriteLine();

            switch (choice.Trim())
            {
                case "1": AddPatient(hospital); break;
                case "2": hospital.PrintWards(); break;
                case "3": hospital.PrintWaiting(); break;
                case "4": DischargePatient(hospital); break;
                case "5": hospital.PrintReport(); break;
                case "6":
                    if (demoDone)
                    {
                        Console.WriteLine("Демо-сценарій уже виконано.");
                    }
                    else
                    {
                        RunDemo(hospital);
                        demoDone = true;
                    }
                    break;
                case "0": running = false; break;
                default: Console.WriteLine("Невідома команда. Введіть число від 0 до 6."); break;
            }
            Console.WriteLine();
        }

        Console.WriteLine("Роботу завершено.");
    }

    // Створює лікарню: 5 лікарів і 4 палати (спочатку реанімації)
    static Hospital CreateHospital()
    {
        var hospital = new Hospital();

        hospital.AddDoctor(new Therapist("Петренко"));
        hospital.AddDoctor(new Therapist("Мельник"));
        hospital.AddDoctor(new Surgeon("Коваль"));
        hospital.AddDoctor(new Surgeon("Бондар"));
        hospital.AddDoctor(new Cardiologist("Шевченко"));

        hospital.AddWard(new IntensiveWard(1, 1));
        hospital.AddWard(new IntensiveWard(2, 1));
        hospital.AddWard(new GeneralWard(3, 2));
        hospital.AddWard(new GeneralWard(4, 2));

        return hospital;
    }

    static void PrintMenu()
    {
        Console.WriteLine("Меню:");
        Console.WriteLine("  1. Додати пацієнта");
        Console.WriteLine("  2. Показати палати");
        Console.WriteLine("  3. Показати чергу очікування");
        Console.WriteLine("  4. Виписати пацієнта з палати");
        Console.WriteLine("  5. Звіт");
        Console.WriteLine("  6. Демо-сценарій");
        Console.WriteLine("  0. Вихід");
        Console.Write("Ваш вибір: ");
    }

    static void AddPatient(Hospital hospital)
    {
        Console.Write("Ім'я пацієнта: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Ім'я не може бути порожнім.");
            return;
        }

        Console.WriteLine("Хвороби:");
        for (int i = 0; i < Disease.Catalog.Count; i++)
            Console.WriteLine($"  {i + 1}. {Disease.Catalog[i]}");

        int? number = ReadNumber("Номер хвороби: ", 1, Disease.Catalog.Count);
        if (number == null) return;

        Console.WriteLine();
        AdmissionResult result = hospital.Admit(name, Disease.Catalog[number.Value - 1]);
        Console.WriteLine(result.Message);
    }

    static void DischargePatient(Hospital hospital)
    {
        hospital.PrintWards();
        Console.Write("Ім'я пацієнта, якого виписати: ");
        string? name = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(name))
        {
            Console.WriteLine("Ім'я не може бути порожнім.");
            return;
        }
        Console.WriteLine();
        hospital.Discharge(name.Trim());
    }

    // Читає число в межах [min, max], поки користувач не введе правильно
    static int? ReadNumber(string prompt, int min, int max)
    {
        while (true)
        {
            Console.Write(prompt);
            string? line = Console.ReadLine();
            if (line == null) return null;
            if (int.TryParse(line.Trim(), out int value) && value >= min && value <= max)
                return value;
            Console.WriteLine($"Введіть число від {min} до {max}.");
        }
    }

    // Готовий сценарій: показує заповнення палат, витіснення, чергу й амбулаторних
    static void RunDemo(Hospital hospital)
    {
        string[,] patients =
        {
            { "Олег", "Пневмонія" },
            { "Ірина", "Перелом кістки" },
            { "Денис", "Гіпертонічний криз" },
            { "Софія", "Перелом кістки" },
            { "Андрій", "Гострий апендицит" },
            { "Марія", "Інфаркт міокарда" },
            { "Богдан", "Гострий апендицит" },
            { "Катерина", "Пневмонія" },
            { "Тарас", "Грип" },
            { "Василь", "Аритмія" },
        };

        for (int i = 0; i < patients.GetLength(0); i++)
        {
            string name = patients[i, 0];
            Disease disease = Disease.Find(patients[i, 1])!;
            Console.WriteLine($">>> {name}: {disease}");
            Console.WriteLine(hospital.Admit(name, disease).Message);
            Console.WriteLine();
        }

        Console.WriteLine("Демо завершено. Виберіть пункт 2 (палати) і 3 (черга),");
        Console.WriteLine("потім пункт 4 і виведіть «Олег»: на його місце зайде пацієнт із черги.");
    }
}
