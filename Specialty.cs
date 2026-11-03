// Перелік (enum): фіксований набір спеціальностей лікарів.
enum Specialty
{
    Therapist,     // терапевт
    Surgeon,       // хірург
    Cardiologist   // кардіолог
}

// Метод розширення: додає до enum метод ToUkrainian() для красивого виводу.
static class SpecialtyExtensions
{
    public static string ToUkrainian(this Specialty specialty)
    {
        switch (specialty)
        {
            case Specialty.Therapist: return "терапевт";
            case Specialty.Surgeon: return "хірург";
            case Specialty.Cardiologist: return "кардіолог";
            default: return specialty.ToString();
        }
    }
}
