// Реанімація: приймає лише найважчих пацієнтів (пріоритет 1).
class IntensiveWard : Ward
{
    public IntensiveWard(int number, int capacity) : base(number, capacity) { }

    public override string Type => "реанімація";

    public override bool CanAccept(Patient patient) => patient.Severity == 1;
}
