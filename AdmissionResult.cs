// Можливі рішення лікарні щодо нового пацієнта
enum AdmissionStatus
{
    Admitted,                 // покладено в палату
    AdmittedWithDisplacement, // покладено в палату, менш терміновий переведений у чергу
    Outpatient,               // палата не потрібна: огляд і додому
    Waiting,                  // зараз не можна, поставлено в чергу очікування
    Rejected                  // прийняти неможливо
}

// Результат спроби прийняти пацієнта: статус + пояснення для користувача
class AdmissionResult
{
    public AdmissionStatus Status { get; }
    public string Message { get; }

    // Чи пацієнта прийнято прямо зараз
    public bool CanBeAdmittedNow =>
        Status != AdmissionStatus.Waiting && Status != AdmissionStatus.Rejected;

    public AdmissionResult(AdmissionStatus status, string message)
    {
        Status = status;
        Message = message;
    }
}
