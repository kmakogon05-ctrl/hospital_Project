// Інтерфейс: «контракт». Усе, що вміє себе описати текстом і надрукувати,
// реалізує Describe() і Print(). Його реалізують Person (а отже Patient і Doctor),
// Ward і WaitingRoom.
// Describe() повертає рядок, тому той самий текст можна і вивести в консоль, і записати у файл.
interface IPrintable
{
    string Describe();
    void Print();
}
