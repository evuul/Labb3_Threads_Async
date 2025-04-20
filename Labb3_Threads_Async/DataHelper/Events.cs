namespace Labb3_Threads_Async.DataHelper;

public static class Events
{
    private static readonly Random random = new();

    public static void CheckForEvent(Car car)
    {
        int eventChance = random.Next(1, 51); // 1 till 50

        if (eventChance <= 1)
        {
            Console.WriteLine($"{car.Name} soppatorsk! Tankar i 15 sekunder.");
            Thread.Sleep(15000);
        }
        else if (eventChance <= 2)
        {
            Console.WriteLine($"{car.Name} fick punktering! Byter däck i 10 sekunder.");
            Thread.Sleep(10000);
        }
        else if (eventChance <= 5)
        {
            Console.WriteLine($"{car.Name} Stenskott på rutan! Byter vindruta 12 sekunders stopp.");
            Thread.Sleep(12000);
        }
        else if (eventChance <= 10)
        {
            car.Speed = Math.Max(10, car.Speed - 5); // Låt inte hastigheten gå under 10
            Console.WriteLine($"{car.Name} har motorproblem! Hastighet sänkt till {car.Speed} km/h.");
        }
    }
}