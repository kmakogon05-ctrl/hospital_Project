using System;
using System.Collections.Generic;

class EmergencyRoom
{
    private readonly PriorityQueue<Patient, (int severity, int seq)> queue = new();
    private int seq = 0;

    public int Waiting => queue.Count;

    public void Register(Patient patient)
    {
        // при однаковому пріоритеті першим іде той, хто прийшов раніше
        queue.Enqueue(patient, (patient.Severity, seq++));
        Console.WriteLine($"Зареєстровано: {patient.Name} (пріоритет {patient.Severity})");
    }

    public Patient? TakeNext()
    {
        if (!queue.TryDequeue(out var patient, out _))
        {
            Console.WriteLine("Черга порожня");
            return null;
        }
        return patient;
    }
}