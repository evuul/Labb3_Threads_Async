namespace Labb3_Threads_Async.DataHelper;

public class Race
{
    private readonly List<Car> cars = new();
    private readonly object lockObject = new();
    private readonly TaskCompletionSource<bool> startSignal = new();
    private int placementCounter = 0;
    private DateTime raceStartTime;
    private bool raceFinished = false;

    public async Task StartRace()
    {
        Console.WriteLine("Välkommen till biltävlingen!");
        Console.WriteLine("Under tävlingen kan du skriva 'status' eller trycka på Enter för att se uppdateringar.");
        Console.WriteLine("Tryck på Enter för att starta tävlingen.");

        AddCars();
        
        // Wait for initial enter press
        Console.ReadLine();

        // Start status checking task
        var statusTask = CheckForStatusInputAsync();

        for (int i = 3; i > 0; i--)
        {
            Console.Clear();
            Console.WriteLine($"🚦 Start om {i}...");
            await Task.Delay(1000);
        }

        Console.Clear();
        Console.WriteLine("🔥 TÄVLINGEN STARTAR NU! VROOOOM! 🔥");
        raceStartTime = DateTime.Now;
        startSignal.SetResult(true);

        // Start all car tasks and wait for completion
        var carTasks = cars.Select(car => CarRaceAsync(car)).ToArray();
        await Task.WhenAll(carTasks);

        Console.WriteLine("\n🏁 Tävlingen är slut! Här är resultaten:");
        string[] medals = { "🥇", "🥈", "🥉" };

        // Print final results
        foreach (var car in cars.OrderBy(c => c.Placement))
        {
            string medal = car.Placement - 1 < 3 ? medals[car.Placement - 1] : $"#{car.Placement}";
            Console.WriteLine($"{medal} {car.Name} - Total tid: {car.TotalTime:F2} sekunder");
        }

        Console.WriteLine("\nTryck på Enter för att avsluta...");
        Console.ReadLine();
    }

    private void AddCars()
    {
        cars.Add(new Car("Volvo V70"));
        cars.Add(new Car("BMW X5"));
        cars.Add(new Car("Mazda MX-5"));
    }

    private async Task CarRaceAsync(Car car)
    {
        await startSignal.Task;

        Console.WriteLine($"{car.Name} startar!");

        while (car.Distance < 5.0)
        {
            car.Distance += car.Speed / 3600.0;
            car.SecondsSinceLastEvent++;

            if (car.SecondsSinceLastEvent >= 10)
            {
                car.SecondsSinceLastEvent = 0;
                Events.CheckForEvent(car);
            }

            if (car.Distance >= 5.0)
            {
                lock (lockObject)
                {
                    placementCounter++;
                    int currentPlacement = placementCounter;
                    double totalTime = (DateTime.Now - raceStartTime).TotalSeconds;

                    car.Placement = currentPlacement;
                    car.TotalTime = totalTime;

                    Console.WriteLine($"{car.Name} har kommit i mål på plats {currentPlacement}.");

                    if (currentPlacement == 1)
                    {
                        Console.WriteLine("👏👏👏 Publiken jublar och applåderar! 👏👏👏");
                    }

                    if (currentPlacement == cars.Count)
                    {
                        raceFinished = true;
                    }
                }
                break;
            }

            await Task.Delay(1000);
        }
    }

    private async Task CheckForStatusInputAsync()
    {
        while (true)
        {
            string input = await Task.Run(() => Console.ReadLine()?.Trim().ToLower());
            if (string.IsNullOrEmpty(input) || input == "status")
            {
                PrintStatus();
            }
        }
    }

    private void PrintStatus()
    {
        Console.WriteLine("\n--- STATUSUPPDATERING ---");
        foreach (var car in cars)
        {
            Console.WriteLine($"{car.Name}: {car.Distance:F2} km, hastighet: {car.Speed} km/h");
        }
        Console.WriteLine("-------------------------\n");
    }
}