public class WaitingRoomTests
{
    [Fact]
    public void Add_KeepsListSortedByUrgency()
    {
        var room = new WaitingRoom();
        var calm = new Patient("Спокійний", TestData.Pneumonia);
        var urgent = new Patient("Терміновий", TestData.Infarct);

        room.Add(calm);
        room.Add(urgent);

        Assert.Same(urgent, room.Patients[0]);
        Assert.Same(calm, room.Patients[1]);
    }

    [Fact]
    public void FindFor_ReturnsMostUrgentPatientTheWardCanAccept()
    {
        var room = new WaitingRoom();
        var severityTwo = new Patient("Двійка", TestData.Fracture);
        var severityOne = new Patient("Одиниця", TestData.Appendicitis);
        room.Add(severityTwo);
        room.Add(severityOne);

        Assert.Same(severityOne, room.FindFor(new IntensiveWard(1, 1)));
    }

    [Fact]
    public void FindFor_NobodySuitable_ReturnsNull()
    {
        var room = new WaitingRoom();
        room.Add(new Patient("Двійка", TestData.Fracture));

        Assert.Null(room.FindFor(new IntensiveWard(1, 1)));
    }

    [Fact]
    public void Remove_AndCount_Work()
    {
        var room = new WaitingRoom();
        var patient = new Patient("Олег", TestData.Pneumonia);
        room.Add(patient);

        Assert.Equal(1, room.Count);
        Assert.True(room.Remove(patient));
        Assert.Equal(0, room.Count);
    }

    [Fact]
    public void PositionOf_IsOneBased()
    {
        var room = new WaitingRoom();
        var first = new Patient("Перший", TestData.Infarct);
        var second = new Patient("Другий", TestData.Pneumonia);
        room.Add(first);
        room.Add(second);

        Assert.Equal(1, room.PositionOf(first));
        Assert.Equal(2, room.PositionOf(second));
    }

    [Fact]
    public void Describe_EmptyQueue()
    {
        Assert.Equal("Черга очікування порожня.", new WaitingRoom().Describe());
    }

    [Fact]
    public void Describe_ListsPatients()
    {
        var room = new WaitingRoom();
        room.Add(new Patient("Олег", TestData.Pneumonia));

        string text = room.Describe();

        Assert.Contains("Черга очікування (1):", text);
        Assert.Contains("1. Пацієнт Олег", text);
    }
}
