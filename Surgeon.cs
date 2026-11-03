using System;

class Surgeon : Doctor
{
    public Surgeon(string name) : base(name, Specialty.Surgeon) { }

    protected override string Action => "оглядає і готує до втручання";

    // Поліморфізм: той самий метод Treat, але хірург додає свою поведінку
    public override void Treat(Patient patient)
    {
        base.Treat(patient);
        if (patient.Severity == 1)
            Console.WriteLine("    Терміново готується операція!");
    }
}
