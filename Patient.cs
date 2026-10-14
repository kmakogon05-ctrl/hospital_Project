class Patient
{
    public string Name { get; }
    public int Severity { get; }          // 1 - найтерміновіше, 5 - найлегше

    public Patient(string name, int severity)
    {
        Name = name;
        Severity = severity;
    }
}