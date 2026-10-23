class Patient : Person
{
    public int Severity { get; }

    public Patient(string name, int severity) : base(name)
    {
        Severity = severity;
    }
}