using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace IN451_Unit6_Justin_Meyer
{
    public sealed class Scenario
    {
        public string Name { get; }
        public int Rooms { get; }
        public int Customers { get; }
        public int NumberOfItems { get; }
        public int MaxItemsPolicy { get; }
        public int SimMinuteMs { get; }
        public int Seed { get; }
        public DateTime StartTime { get; private set; }
        public DateTime EndTime { get; private set; }
        public TimeSpan Elapsed => EndTime - StartTime;

        public Scenario(string name, int rooms, int customers, int numberOfItems = 0, int maxItemsPolicy = 6, int simMinuteMs = 100, int? seed = null)
        {
            Name = name;
            Rooms = rooms;
            Customers = customers;
            NumberOfItems = numberOfItems;
            MaxItemsPolicy = Math.Max(1, maxItemsPolicy);
            SimMinuteMs = Math.Max(1, simMinuteMs);
            Seed = seed ?? Environment.TickCount;
        }

        public ScenarioResult Run()
        {
            StartTime = DateTime.Now;

            var rng = new Random(Seed);
            var dressingRooms = new DressingRooms(Rooms);

            var results = new ConcurrentBag<CustomerResult>();
            var threads = new List<Thread>(Customers);

            for (int i = 1; i <= Customers; i++)
            {
                var customer = new Customer(i, dressingRooms, NumberOfItems, MaxItemsPolicy, rng, SimMinuteMs);
                var t = new Thread(() =>
                {
                    customer.Run();
                    results.Add(customer.Result);
                });
                t.IsBackground = true;
                threads.Add(t);
            }

            foreach (var t in threads) t.Start();
            foreach (var t in threads) t.Join();

            EndTime = DateTime.Now;

            var resultList = results.ToList();
            double avgItems = resultList.Count > 0 ? resultList.Average(r => r.ItemCount) : 0;
            double avgUse = resultList.Count > 0 ? resultList.Average(r => r.UseTimeSimMinutes) : 0;
            double avgWait = resultList.Count > 0 ? resultList.Average(r => r.WaitTimeSimMinutes) : 0;

            var sr = new ScenarioResult
            {
                Name = Name,
                StartTime = StartTime,
                EndTime = EndTime,
                Elapsed = Elapsed,
                Rooms = Rooms,
                Customers = Customers,
                NumberOfItems = NumberOfItems,
                MaxItemsPolicy = MaxItemsPolicy,
                SimMinuteMs = SimMinuteMs,
                Seed = Seed,
                AverageItems = avgItems,
                AverageUseMinutes = avgUse,
                AverageWaitMinutes = avgWait,
                Results = resultList
            };

            return sr;
        }

        public static ScenarioResult Scenario01() => 
            new Scenario("scenario01", rooms: 3, customers: 20, numberOfItems: 0, maxItemsPolicy: 6, simMinuteMs: 100, seed: 101).Run();

        public static ScenarioResult Scenario02() => 
            new Scenario("scenario02", rooms: 2, customers: 30, numberOfItems: 0, maxItemsPolicy: 6, simMinuteMs: 100, seed: 202).Run();

        public static ScenarioResult Scenario03() => 
            new Scenario("scenario03", rooms: 5, customers: 30, numberOfItems: 0, maxItemsPolicy: 6, simMinuteMs: 100, seed: 303).Run();
    }

    public sealed class ScenarioResult
    {
        public string Name { get; set; } = "";
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Elapsed { get; set; }
        public int Rooms { get; set; }
        public int Customers { get; set; }
        public int NumberOfItems { get; set; }
        public int MaxItemsPolicy { get; set; }
        public int SimMinuteMs { get; set; }
        public int Seed { get; set; }

        public double AverageItems { get; set; }
        public double AverageUseMinutes { get; set; }
        public double AverageWaitMinutes { get; set; }
        public List<CustomerResult> Results { get; set; } = new();

        public override string ToString()
        {
            return $@"
== {Name} ==
Rooms: {Rooms}, Customers: {Customers}, ForcedItems: {NumberOfItems} (0=random, <=Max {MaxItemsPolicy})
Sim Minute = {SimMinuteMs} ms, Seed = {Seed}
Start: {StartTime}, End: {EndTime}, Elapsed: {Elapsed.TotalSeconds:F2}s
Average items per customer: {AverageItems:F2}
Average room usage time: {AverageUseMinutes:F2} minutes (simulated)
Average wait time for room: {AverageWaitMinutes:F2} minutes (simulated)
";
        }
    }
}
