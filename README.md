# hospital_Project

Лабораторна робота 3: розширення лабораторної 2 (лікарня: палати, лікарі, черга).

## Що додано
- **Логування** (`Logging.cs`): інтерфейс `ILogger`, `FileLogger` (пише у `logs/hospital.log`), `NullLogger`.
  `Hospital` приймає логер через конструктор і логує прийом, виписку, відмови та помилки файлів.
- **Введення з файлу** (`PatientFileReader.cs`, пункт меню 7): пацієнти читаються з `patients.txt`
  у форматі `Ім'я;Хвороба`. Некоректні рядки пропускаються й потрапляють у журнал.
- **Виведення у файл** (`Hospital.BuildReport` / `SaveReport`, пункт меню 8): звіт зберігається в `report.txt`.
- **Юніт-тести** (`hospital_Project.Tests`, xUnit).

## Запуск
```
dotnet run
```

## Тести
```
dotnet test hospital_Project.Tests
```
