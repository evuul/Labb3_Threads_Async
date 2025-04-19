namespace Labb3_Threads_Async.DataHelper;

public static class Events
{
    private static readonly Random random = new();

    public static void CheckForEvent(Car car)
    {
        int eventChance = random.Next(1, 51); // 1 till 50

        if (eventChance <= 1)
        {
            Console.WriteLine($"{car.Name} fick slut på bensin! Tankar i 15 sekunder.");
            Thread.Sleep(15000);
        }
        else if (eventChance <= 3)
        {
            Console.WriteLine($"{car.Name} fick punktering! Byter däck i 10 sekunder.");
            Thread.Sleep(10000);
        }
        else if (eventChance <= 8)
        {
            Console.WriteLine($"{car.Name} fick fågel på vindrutan! Tvättar i 5 sekunder.");
            Thread.Sleep(5000);
        }
        else if (eventChance <= 18)
        {
            car.Speed = Math.Max(10, car.Speed - 1); // Låt inte hastigheten gå under 10
            Console.WriteLine($"{car.Name} har motorproblem! Hastighet sänkt till {car.Speed} km/h.");
        }
    }
}