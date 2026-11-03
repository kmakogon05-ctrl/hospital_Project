using System;
using System.Collections.Generic;
using System.Linq;

// Лікарня: об'єднує палати, лікарів і чергу очікування та ухвалює рішення,
// чи можна прийняти пацієнта.
class Hospital
{
    private readonly List<Ward> wards = new();
    private readonly List<Doctor> doctors = new();
    private readonly WaitingRoom waiting = new();
    private readonly HashSet<string> inHospital = new(StringComparer.OrdinalIgnoreCase);
    private readonly List<string> discharged = new();

    public void AddWard(Ward ward) => wards.Add(ward);
    public void AddDoctor(Doctor doctor) => doctors.Add(doctor);

    // ---------- Прийом пацієнта (перевантаження: два варіанти виклику) ----------

    public AdmissionResult Admit(string name, Disease disease)
    {
        Patient patient;
        try
        {
            patient = new Patient(name, disease);
        }
        catch (ArgumentException ex)
        {
            return new AdmissionResult(AdmissionStatus.Rejected, "НЕ МОЖНА прийняти: " + ex.Message);
        }
        return Admit(patient);
    }

    public AdmissionResult Admit(Patient patient)
    {
        // 1. Перевірки: чи взагалі можна прийняти
        if (inHospital.Contains(patient.Name))
            return new AdmissionResult(AdmissionStatus.Rejected,
                $"НЕ МОЖНА прийняти: пацієнт «{patient.Name}» уже є в лікарні.");

        if (PickDoctor(patient.Disease) == null)
            return new AdmissionResult(AdmissionStatus.Rejected,
                $"НЕ МОЖНА прийняти: у лікарні немає лікаря-{patient.Disease.RequiredSpecialty.ToUkrainian()}а для хвороби «{patient.Disease.Name}».");

        inHospital.Add(patient.Name);

        // 2. Легкий стан (3-5): палата не потрібна
        if (patient.Severity >= 3)
        {
            StartTreatment(patient);
            inHospital.Remove(patient.Name);
            discharged.Add(patient.Name);
            return new AdmissionResult(AdmissionStatus.Outpatient,
                $"МОЖНА прийняти: «{patient.Disease.Name}» (пріоритет {patient.Severity}) не потребує палати. Огляд проведено, пацієнта відправлено додому.");
        }

        // 3. Важкий стан (1-2): потрібне ліжко. Спочатку шукаємо вільне
        Ward? freeWard = wards.FirstOrDefault(w => w.HasSpace && w.CanAccept(patient));
        if (freeWard != null)
        {
            freeWard.TryAdmit(patient);
            StartTreatment(patient);
            return new AdmissionResult(AdmissionStatus.Admitted,
                $"МОЖНА прийняти: пацієнта покладено в палату {freeWard.Number} ({freeWard.Type}).");
        }

        // 4. Вільних місць немає: чи є менш терміновий пацієнт, якого можна перемістити?
        var victim = FindVictim(patient);
        if (victim != null)
        {
            Ward ward = victim.Value.Ward;
            Patient displaced = victim.Value.Patient;

            ward.Discharge(displaced);       // звільняємо ліжко
            waiting.Add(displaced);          // менш терміновий чекає в черзі
            ward.TryAdmit(patient);          // терміновий займає ліжко
            StartTreatment(patient);

            return new AdmissionResult(AdmissionStatus.AdmittedWithDisplacement,
                $"МОЖНА прийняти: пацієнта покладено в палату {ward.Number} ({ward.Type}). " +
                $"Вільних місць не було, тому менш терміновий пацієнт {displaced.Name} " +
                $"({displaced.Disease.Name}, пріоритет {displaced.Severity}) переведений у чергу очікування.");
        }

        // 5. Нікого не можна посунути: пацієнт чекає
        waiting.Add(patient);
        return new AdmissionResult(AdmissionStatus.Waiting,
            $"Прийняти ЗАРАЗ не можна: вільних місць немає, а менш термінових пацієнтів у палатах немає. " +
            $"Пацієнта поставлено в чергу очікування, позиція {waiting.PositionOf(patient)}.");
    }

    // ---------- Виписка ----------

    public bool Discharge(string name)
    {
        foreach (Ward ward in wards)
        {
            Patient? patient = ward.Discharge(name);
            if (patient == null) continue;

            inHospital.Remove(patient.Name);
            discharged.Add(patient.Name);
            Console.WriteLine($"Виписано: {patient.Name} із палати {ward.Number}. Місце звільнилось.");

            // На звільнене місце йде найтерміновіший із черги, кого ця палата може прийняти
            Patient? next = waiting.FindFor(ward);
            if (next != null)
            {
                waiting.Remove(next);
                ward.TryAdmit(next);
                Console.WriteLine($"З черги на це місце переведено: {next.Name} " +
                                  $"({next.Disease.Name}, пріоритет {next.Severity}) -> палата {ward.Number}.");
                StartTreatment(next);
            }
            else
            {
                Console.WriteLine("У черзі немає пацієнтів, яких можна покласти в цю палату.");
            }
            return true;
        }

        if (waiting.Patients.Any(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
            Console.WriteLine($"«{name}» ще чекає в черзі й не лежить у палаті, виписувати нікого.");
        else
            Console.WriteLine($"Пацієнта «{name}» немає в палатах.");
        return false;
    }

    // ---------- Виведення (поліморфізм через IPrintable) ----------

    public void PrintWards()
    {
        Console.WriteLine("Палати:");
        PrintAll(wards);
    }

    public void PrintWaiting() => waiting.Print();

    public void PrintReport()
    {
        Console.WriteLine("=== Звіт ===");
        PrintWards();
        waiting.Print();
        Console.WriteLine("Лікарі:");
        PrintAll(doctors);
        Console.WriteLine("Виписано: " + (discharged.Count > 0 ? string.Join(", ", discharged) : "ніхто"));
    }

    // Приймає будь-які об'єкти, що реалізують IPrintable: палати, лікарів...
    private static void PrintAll(IEnumerable<IPrintable> items)
    {
        foreach (IPrintable item in items)
            item.Print();
    }

    // ---------- Допоміжні методи ----------

    // Лікар потрібної спеціальності з найменшим навантаженням
    private Doctor? PickDoctor(Disease disease) =>
        doctors.Where(d => d.CanTreat(disease)).OrderBy(d => d.Treated.Count).FirstOrDefault();

    // Лікар приймає пацієнта лише один раз (навіть якщо пацієнта переміщували)
    private void StartTreatment(Patient patient)
    {
        if (patient.AttendingDoctor != null) return;
        Doctor? doctor = PickDoctor(patient.Disease);
        if (doctor == null) return;
        doctor.Treat(patient);
        patient.AssignDoctor(doctor);
    }

    // Найменш терміновий пацієнт у палатах, яка підходить новачку,
    // і якщо він МЕНШ терміновий за новачка (пріоритет рівний не посуваємо)
    private (Ward Ward, Patient Patient)? FindVictim(Patient newcomer)
    {
        var candidates = wards
            .Where(w => w.CanAccept(newcomer))
            .SelectMany(w => w.Patients
                .Where(p => p.Severity > newcomer.Severity)
                .Select(p => (Ward: w, Patient: p)))
            .OrderByDescending(x => x.Patient.Severity)   // найменш терміновий
            .ThenByDescending(x => x.Patient.Id)          // а з рівних той, хто прийшов пізніше
            .ToList();

        if (candidates.Count == 0) return null;
        return candidates[0];
    }
}
