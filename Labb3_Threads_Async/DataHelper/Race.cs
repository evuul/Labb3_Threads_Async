using System.Collections.Concurrent;

namespace Labb3_Threads_Async.DataHelper;

public class Race
{
    private readonly List<Car> cars = new();
    private readonly object lockObject = new();
    private readonly ManualResetEvent startSignal = new(false);
    private readonly List<Thread> threads = new();
    private int placementCounter = 0;
    private DateTime raceStartTime;
    private bool raceFinished = false;

    public void StartRace()
    {
        Console.WriteLine("Välkommen till biltävlingen!");
        Console.WriteLine("Du kan skriva 'status' eller trycka på enter för uppdateringar.");

        AddCars();

        foreach (var car in cars)
        {
            Thread carThread = new(() => CarRace(car));
            threads.Add(carThread);
            carThread.Start();
        }

        Thread statusThread = new(CheckForStatusInput) { IsBackground = true };
        statusThread.Start();

        Console.WriteLine("Tryck på Enter för att starta tävlingen...");
        Console.ReadLine();

        for (int i = 3; i > 0; i--)
        {
            Console.Clear();
            Console.WriteLine($"🚦 Start om {i}...");
            Thread.Sleep(1000);
        }

        Console.Clear();
        Console.WriteLine("🔥 TÄVLINGEN STARTAR NU! VROOOOM! 🔥");
        raceStartTime = DateTime.Now; // Spara starttiden
        startSignal.Set();

        foreach (var thread in threads)
        {
            thread.Join();
        }

        Console.WriteLine("\n🏁 Tävlingen är slut! Här är resultaten:");
        string[] medals = { "🥇", "🥈", "🥉" };

        // Skriv ut slutresultat med medaljsymboler
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

    private void CarRace(Car car)
    {
        startSignal.WaitOne();

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

                    // Sätt värdena i Car-objektet
                    car.Placement = currentPlacement;
                    car.TotalTime = totalTime;

                    // Skriv ut när bilen går i mål
                    Console.WriteLine($"{car.Name} har kommit i mål på plats {currentPlacement}.");
                    
                    // Lägg till publikens jubel för första platsen
                    if (currentPlacement == 1)
                    {
                        Console.WriteLine("👏👏👏 Publiken jublar och applåderar! 👏👏👏");
                    }

                    // Markera tävlingen som avslutad när alla bilar gått i mål
                    if (currentPlacement == cars.Count)
                    {
                        raceFinished = true;
                    }
                }
                break;
            }

            Thread.Sleep(1000);
        }
    }

    private void CheckForStatusInput()
    {
        while (true)
        {
            string input = Console.ReadLine()?.Trim().ToLower();
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