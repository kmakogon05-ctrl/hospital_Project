class Therapist : Doctor
{
    public Therapist(string name) : base(name, Specialty.Therapist) { }

    protected override string Action => "проводить огляд і призначає лікування";
}
