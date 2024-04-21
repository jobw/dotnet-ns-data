using System.Diagnostics.Tracing;
using System.Globalization;

namespace NsDataTest
{
    internal class Program
    {
        static bool dataInitiated = false;
        static DateOnly? showForDate { get; set; }

        static void Main(string[] args)
        {
            Console.CancelKeyPress += (s, e) => { Console.Clear(); Main(args); };            

            while (true)
            {
                Console.Clear();

                if (!dataInitiated)
                {
                    Console.WriteLine("Data not yet initialized. \nTo start initiating the program press any key.");
                    Console.ReadKey();
                    Console.WriteLine("\nInitiating timetable data...");
                    try
                    {
                        System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Journey).TypeHandle);
                    }
                    catch (TypeInitializationException ex) {
                        if (ex.InnerException == null) throw ex;
                        while (true)
                            if (ex.InnerException is FileNotFoundException)
                            {
                                Console.WriteLine("\nDataset not found!\n" +
                                    "Get the dataset from: https://reisinformatiegroep.nl/ndovloket/Data/ShowDataCollection/2 \n" +
                                    $"Create a folder called \"Dataset\" in the following directory: {Environment.CurrentDirectory} \n" +
                                    "Unpack the data and place it in the newly created folder and restart the program." +
                                    $"located in " +
                                    $"\n Press any key to close the program.");
                                Console.ReadKey(true);
                                return;
                            }                                
                            else if (ex.InnerException is TypeInitializationException)
                                continue;
                            else
                                throw;
                    }

                    Console.Clear();
                    Console.WriteLine($"Data initialized!\n");

                    dataInitiated = true;
                }

                showForDate = DateTime.Now.Date.CompareTo(Publication.ValidityEndDate.ToDateTime(new TimeOnly())) > 1 ? DateOnly.FromDateTime(DateTime.Now) : Publication.ValidityEndDate;

                while (true)
                {
                    Console.WriteLine("Welcome!\n" +
                        "This program allows you to search through the timetable of the Dutch national railwyas (NS).");
                    Console.WriteLine($"Loaded timetable valid from: {Publication.ValidityStartDate}, until {Publication.ValidityEndDate}.");
                    Console.WriteLine("\nInteract with the timetable: (press key to navigate to the option next to it)" +
                        "\nTo return to this top menu, press [Ctrl + C].\n" +
                        "\n1: Search routes by Id." +
                        "\n2: Find departures by time." +
                        "\n3: Simulate timetable" +
                        "\n\n0: Options..." +
                        "\n\n[Esc] to close the program.");

                    switch (Console.ReadKey().Key)
                    {
                        case ConsoleKey.Escape:
                            Environment.Exit(0);
                            break;
                        case ConsoleKey.D1 or ConsoleKey.NumPad1:
                            SearchRouteOption();
                            break;
                        case ConsoleKey.D2 or ConsoleKey.NumPad2:
                            SearchByDepartureTimeOption();
                            break;
                        case ConsoleKey.D3 or ConsoleKey.NumPad3:
                            SimulateTimetableOption();
                            break;
                        default:
                            Console.Clear();
                            continue;
                    }                    
                }
            }            
        }

        static void SearchRouteOption()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("To return to the top menu, press [Ctrl + C].\n\n");

                Console.WriteLine("Search for a route by its route id." +
                    $"\nEnter the Id of the route: (integer from 1 to {Journey.AllJourneys.Count})");
                try
                {
                    Console.WriteLine(Journey.GenerateItinerary(uint.Parse(Console.ReadLine() ?? throw new Exception("Input was empty"))));
                }
                catch (FormatException ex) { Console.WriteLine(ex.Message); }
                catch (Exception ex) { Console.WriteLine(ex.Message); }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        static void SearchByDepartureTimeOption()
        {
            while (true)
            {
                Console.Clear();

                Console.WriteLine("Find all trains departing at a given time.");

                Console.WriteLine($"\nBy default, journeys are shown for today's " +
                    $"({DateOnly.FromDateTime(DateTime.Now.Date)}) date." +
                    $"\n(when todays date is outside of the validity of the provided timetable " +
                    $"the last day of the timetable will be used)" +
                    "\n\nEnter time: (HH:mm)");
                Console.WriteLine();
                try
                {
                    if (Journey.TryGetJourneysFromDepartureTime(
                                    TimeOnly.ParseExact(Console.ReadLine() ?? throw new Exception("Input was empty"), "HH:mm", CultureInfo.InvariantCulture), 
                                    out var l, 
                                    DateOnly.FromDateTime(DateTime.Now))
                        )
                        foreach (var journey in l)
                            Console.WriteLine($"[{journey.Value.Id}] {journey.Key.Station?.FullName} : " +
                                $"({journey.Value.TrainJourneys[0].Company.Name.Trim()}: " +
                                $"{journey.Value.TransitTypes[0].Description}) " +
                                $"{journey.Value.Stops.First().Station?.FullName} --> {journey.Value.Stops.Last().Station?.FullName}");
                }
                catch (FormatException) 
                {
                    Console.WriteLine("Input was not a valid time!");
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); return; }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }
        }

        static void SimulateTimetableOption() 
        {
            Console.WriteLine("Not yet implemented!");

            Console.WriteLine("\nPress any key to continue...");
            Console.ReadKey();

            Console.Clear();

            return;
        }
    }
}