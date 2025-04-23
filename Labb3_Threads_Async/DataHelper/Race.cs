namespace Labb3_Threads_Async.DataHelper;

public class Race
{
    private readonly List<Car> cars = new();
    private readonly object lockObject = new(); // Lock object for thread safety
    private readonly TaskCompletionSource<bool> startSignal = new();
    private int placementCounter = 0;
    private DateTime raceStartTime;
    private bool raceFinished = false;
    private double RaceDistance; // Distans för tävling

    public async Task StartRace()
    {
        Console.WriteLine("Välkommen till biltävlingen!");

        double inputDistance = 0;
        
        // Användare anger tävlingsdistans
        while (true)
        {
            Console.WriteLine("Hur lång ska tävlingen vara? (Mellan 1 och 10 km):");

            if (double.TryParse(Console.ReadLine(), out inputDistance) && inputDistance >= 1 && inputDistance <= 10)
            {
                break;  
            }
            
            Console.WriteLine("Ogiltigt värde! Vänligen ange ett heltal mellan 1 och 10.");
        }

        RaceDistance = inputDistance; // sätter distans

        AddCars();

        Console.WriteLine("\nUnder tävlingen kan du skriva 'status' eller trycka på Enter för att se uppdateringar.");
        Console.WriteLine("Tryck på Enter för att starta tävlingen.");
        Console.ReadLine();
        
        var statusTask = CheckForStatusInputAsync();

        // Nedräkning innan starten
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

        // Kör bilarna på separata trådar
        var carTasks = cars.Select(car => CarRaceAsync(car)).ToArray();
        await Task.WhenAll(carTasks);
        
        raceFinished = true; // Tävlingen är över, sätt flaggan

        // Resultat
        Console.WriteLine("\n🏁 Tävlingen är slut! Här är resultaten:");
        string[] medals = { "🥇", "🥈", "🥉" };

        foreach (var car in cars.OrderBy(c => c.Placement))
        {
            string medal = car.Placement - 1 < 3 ? medals[car.Placement - 1] : $"#{car.Placement}";
            Console.WriteLine($"{medal} {car.Name} - Total tid: {car.TotalTime:F2} sekunder");
        }

        // Nedräkning och stäng programmet
        await CountdownToExit();
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

        while (car.Distance < RaceDistance)
        {
            double distanceToMove = car.Speed / 3600.0; // Hur långt bilen kör per sekund
            
            if (car.Distance + distanceToMove > RaceDistance)
            {
                car.Distance = RaceDistance;
            }
            else
            {
                car.Distance += distanceToMove; 
            }

            car.SecondsSinceLastEvent++;

            if (car.SecondsSinceLastEvent >= 10)
            {
                car.SecondsSinceLastEvent = 0;
                await Events.CheckForEvent(car); // Eventhantering var 10e sekund
            }

            if (car.Distance >= RaceDistance)
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
                    
                    // Kontrollera om alla bilar har kommit i mål
                    if (currentPlacement == cars.Count) 
                    {
                        raceFinished = true;
                    }
                }
                break;
            }

            await Task.Delay(1000); // Vänta en sekund
        }
    }
    // lyssnar på enter eller stats
    private async Task CheckForStatusInputAsync()
    {
        while (!raceFinished) // Avsluta när tävlingen är slut
        {
            string input = await Task.Run(() => Console.ReadLine()?.Trim().ToLower());
            if (string.IsNullOrEmpty(input) || input == "status")
            {
                PrintStatus();
            }
        }
    }
    // skriver ut status för bilarna
    private void PrintStatus()
    {
        Console.WriteLine("\n--- STATUSUPPDATERING ---");
        foreach (var car in cars)
        {
            Console.WriteLine($"{car.Name}: {car.Distance:F2} km, hastighet: {car.Speed} km/h");
        }
        Console.WriteLine("-------------------------\n");
    }

    // Nedräkning och stängning av programmet
    private async Task CountdownToExit()
    {
        Console.WriteLine("\nTävlingen är avslutad. Stänger programmet om:");
        
        for (int i = 5; i > 0; i--)
        {
            Console.WriteLine(i);
            await Task.Delay(1000);
        }

        Console.WriteLine("Avslutar programmet...");
        Environment.Exit(0);
    }
}