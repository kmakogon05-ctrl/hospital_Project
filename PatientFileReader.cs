using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

// Один успішно прочитаний рядок файлу: ім'я + хвороба з довідника
class PatientEntry
{
    public string Name { get; }
    public Disease Disease { get; }

    public PatientEntry(string name, Disease disease)
    {
        Name = name;
        Disease = disease;
    }
}

// Результат читання: що вдалося розпізнати і які рядки були помилковими
class PatientImport
{
    public List<PatientEntry> Entries { get; } = new();
    public List<string> Errors { get; } = new();
}

// Читає пацієнтів із текстового файлу. Формат рядка:  Ім'я;Хвороба
// Порожні рядки та рядки, що починаються з #, пропускаються.
static class PatientFileReader
{
    public const char Separator = ';';

    // Читає файл. Якщо файлу немає, кидає FileNotFoundException (обробляє той, хто викликає).
    public static PatientImport ReadFile(string path)
    {
        if (!File.Exists(path))
            throw new FileNotFoundException($"Файл «{path}» не знайдено.", path);

        return Parse(File.ReadAllLines(path, Encoding.UTF8));
    }

    // Розбір рядків окремо від файлу, щоб його було легко тестувати
    public static PatientImport Parse(IEnumerable<string> lines)
    {
        var result = new PatientImport();
        int lineNumber = 0;

        foreach (string rawLine in lines)
        {
            lineNumber++;
            string line = rawLine.Trim();

            if (line.Length == 0 || line.StartsWith('#'))
                continue;

            string[] parts = line.Split(Separator);
            if (parts.Length != 2 ||
                string.IsNullOrWhiteSpace(parts[0]) ||
                string.IsNullOrWhiteSpace(parts[1]))
            {
                result.Errors.Add($"Рядок {lineNumber}: очікується формат «Ім'я{Separator}Хвороба», а є «{line}».");
                continue;
            }

            string name = parts[0].Trim();
            string diseaseName = parts[1].Trim();

            Disease? disease = Disease.Find(diseaseName);
            if (disease == null)
            {
                result.Errors.Add($"Рядок {lineNumber}: невідома хвороба «{diseaseName}».");
                continue;
            }

            result.Entries.Add(new PatientEntry(name, disease));
        }

        return result;
    }
}
