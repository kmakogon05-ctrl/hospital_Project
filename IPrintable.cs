// Інтерфейс: «контракт». Усе, що вміє себе надрукувати, реалізує метод Print().
// Його реалізують Person (а отже Patient і Doctor), Ward і WaitingRoom.
interface IPrintable
{
    void Print();
}
