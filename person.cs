using System;

// Абстрактний клас: спільна основа для Patient і Doctor.
// Створити об'єкт Person напряму (new Person(...)) не можна.
abstract class Person : IPrintable
{
    public string Name { get; }

    // protected: конструктор можуть викликати лише нащадки
    protected Person(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Ім'я не може бути порожнім.");
        Name = name.Trim();
    }

    // Абстрактна властивість: кожен нащадок ОБОВ'ЯЗКОВО її реалізує
    public abstract string Role { get; }

    // Віртуальний метод: нащадки МОЖУТЬ його перевизначити (override)
    public virtual string Describe() => $"{Role}: {Name}";

    // Реалізація інтерфейсу IPrintable
    public void Print() => Console.WriteLine(Describe());

    public override string ToString() => Describe();
}
