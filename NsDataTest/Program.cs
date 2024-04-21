using System.Globalization;
using System.Net.Http.Headers;
using System.Xml.Serialization;

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
                    Console.WriteLine("Initiating timetable data...");
                    System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Journey).TypeHandle);
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
                        "\nTo return to this top menu, press [Ctrl + C]." +
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
                        case ConsoleKey.D1:
                            SearchRouteOption();
                            break;
                        case ConsoleKey.D2:
                            SearchByDepartureTimeOption();
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
                    Journey journey = Journey.GetById(uint.Parse(Console.ReadLine()));
                    Console.WriteLine($"\nJourney for {journey.TrainJourneys[0].Company.Name.Trim()}: {journey.TransitTypes[0].Description} [{journey.TransitTypes[0].Code}]\n");
                    if (journey.Attributes.Count > 0)
                    {
                        Console.WriteLine("This journey has the following attributes: ");
                        foreach (JourneyAttribute attribute in journey.Attributes) 
                            Console.WriteLine($"From {journey.Stops[attribute.StartStop].Station?.FullName} until {journey.Stops[attribute.EndStop].Station?.FullName}: \n" +
                                $" > {attribute.Attribute.Description}");
                        Console.WriteLine();
                    }
                    foreach (Stop stop in journey.Stops) 
                    {
                        if (stop is not FirstStop and not TracklessFirstStop)
                            Console.WriteLine("|");
                        switch (stop)
                        {
                            case FirstStop _stop: Console.WriteLine($" / {_stop.DepartureTime} - {_stop.Station?.FullName} (track: {_stop.DepartureTrack.TrackString})"); break;
                            case TracklessFirstStop _stop: Console.WriteLine($" / {_stop.DepartureTime} - {_stop.Station?.FullName} (track unavailable)"); break;
                            case ShortStop _stop: Console.WriteLine($" > {_stop.ArrivalAndDepartureWithinMinuteOf} - {_stop.Station?.FullName} (track: {_stop.DepartureTrack.TrackString})"); break;
                            case TracklessShortStop _stop: Console.WriteLine($" > {_stop.ArrivalAndDepartureWithinMinuteOf} - {_stop.Station?.FullName} (track unavailable)"); break;
                            case PassingStop _stop: Console.WriteLine($"| {_stop.Station?.FullName }"); break;
                            case RegularStop _stop: Console.WriteLine($" \\ {_stop.ArrivalTime} (track: {_stop.ArrivalTrack.TrackString} \n  | {_stop.Station?.FullName} \n / {_stop.DepartureTime} (track: {_stop.DepartureTrack.TrackString})"); break;
                            case TracklessRegularStop _stop: Console.WriteLine($" \\ {_stop.ArrivalTime} (track unavailable) \n  | {_stop.Station?.FullName} \n / {_stop.DepartureTime} (track unavailable)"); break;
                            case TracklessTerminusStop _stop: Console.WriteLine($" \\ {_stop.ArrivalTime} - {_stop.Station?.FullName} (track unavailable)"); break;
                            case TerminusStop _stop: Console.WriteLine($" \\ {_stop.ArrivalTime} - {_stop.Station?.FullName} (track: {_stop.ArrivalTrack.TrackString})"); break;
                        }
                        if (stop is not TerminusStop and not TracklessTerminusStop)
                            Console.WriteLine("|");
                    }
                }
                catch (FormatException ex) { Console.WriteLine(ex.Data[0]); }
                catch (Exception ex) { Console.WriteLine(ex.Message); return; }

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

                Console.WriteLine($"\nBy default, journeys are shown for today's ({DateOnly.FromDateTime(DateTime.Now.Date)}) date." +
                    $"\n(when todays date is outside of the validity of the provided timetable the last day of the timetable will be used)" +
                    "\n\nEnter time: (HH:mm)");
                Console.WriteLine();
                try
                {
                    if (Journey.TryGetJourneysFromDepartureTime(TimeOnly.ParseExact(Console.ReadLine(), "HH:mm", CultureInfo.InvariantCulture), out var l, DateOnly.FromDateTime(DateTime.Now)))
                        foreach (var journey in l)
                            Console.WriteLine($"[{journey.Value.Id}] {journey.Key.Station?.FullName} : ({journey.Value.TrainJourneys[0].Company.Name.Trim()}: {journey.Value.TransitTypes[0].Description}) {journey.Value.Stops.First().Station?.FullName} --> {journey.Value.Stops.Last().Station?.FullName}");
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
    }
}