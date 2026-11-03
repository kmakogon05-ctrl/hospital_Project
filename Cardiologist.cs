class Cardiologist : Doctor
{
    public Cardiologist(string name) : base(name, Specialty.Cardiologist) { }

    protected override string Action => "знімає кардіограму й оцінює стан серця";
}
