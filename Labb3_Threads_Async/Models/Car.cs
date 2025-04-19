namespace Labb3_Threads_Async;

public class Car
{
    public string Name { get; set; }
    public double Distance { get; set; }
    public int Speed { get; set; }
    public int SecondsSinceLastEvent { get; set; }
    public int Placement { get; set; }
    public double TotalTime { get; set; }

    public Car(string name)
    {
        Name = name;
        Distance = 0;
        Speed = 120; // 120 km/h starthastighet
        SecondsSinceLastEvent = 0;
        Placement = 0;
        TotalTime = 0.0;
    }
}