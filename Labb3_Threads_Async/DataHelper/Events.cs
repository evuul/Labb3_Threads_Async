using System.Threading.Tasks;

namespace Labb3_Threads_Async.DataHelper;

public static class Events
{
    private static readonly Random random = new();

    public static async Task CheckForEvent(Car car)
    {
        int eventChance = random.Next(1, 51); // 1 till 50

        if (eventChance <= 3)
        {
            Console.WriteLine($"{car.Name} soppatorsk! Tankar i 15 sekunder.");
            await Task.Delay(15000);
        }
        else if (eventChance <= 5)
        {
            Console.WriteLine($"{car.Name} fick punktering! Byter däck i 10 sekunder.");
            await Task.Delay(10000);
        }
        else if (eventChance <= 8)
        {
            Console.WriteLine($"{car.Name} stenskott på rutan! Byter vindruta – 12 sekunders stopp.");
            await Task.Delay(12000);
        }
        else if (eventChance <= 15)
        {
            car.Speed = Math.Max(10, car.Speed - 5); // Minsta hastighet 10 km/h
            Console.WriteLine($"{car.Name} har motorproblem! Hastighet sänkt till {car.Speed} km/h.");
        }
    }
}