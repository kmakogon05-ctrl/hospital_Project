// Звичайна палата: приймає пацієнтів із пріоритетом 1 або 2.
class GeneralWard : Ward
{
    public GeneralWard(int number, int capacity) : base(number, capacity) { }

    public override string Type => "звичайна";

    public override bool CanAccept(Patient patient) => patient.Severity <= 2;
}
