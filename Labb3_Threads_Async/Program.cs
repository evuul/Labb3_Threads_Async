using Labb3_Threads_Async.DataHelper;

namespace Labb3_Threads_Async;

class Program
{
    static async Task Main(string[] args)
    {
        var race = new Race();
        race.StartRace();
    }
}