using System;
using System.Diagnostics;
using System.Threading;

namespace IN451_Unit6_Justin_Meyer
{

    public sealed class Customer
    {
        private readonly int _id;
        private readonly DressingRooms _rooms;
        private readonly int _forcedItems;
        private readonly int _maxItemsPolicy;
        private readonly Random _rng;
        private readonly int _simMinuteMs;

        public CustomerResult Result { get; set; }

        public Customer(int id, DressingRooms rooms, int forcedItems, int maxItemsPolicy, Random rng, int simMinuteMs = 100)
        {
            _id = id;
            _rooms = rooms;
            _forcedItems = forcedItems;
            _maxItemsPolicy = Math.Max(1, maxItemsPolicy);
            _rng = rng;
            _simMinuteMs = simMinuteMs;
            Result = new CustomerResult { CustomerId = id };
        }

        public void Run()
        {
            int items;
            if (_forcedItems == 0)
            {
                items = _rng.Next(1, _maxItemsPolicy + 1);
            }
            else
            {
                items = Math.Min(Math.Max(_forcedItems, 1), _maxItemsPolicy);
            }

            Result.ItemCount = items;

            var waitSw = Stopwatch.StartNew();
            using (_rooms.RequestRoom())
            {
                waitSw.Stop();
                Result.WaitTimeSimMinutes = waitSw.ElapsedMilliseconds / (double)_simMinuteMs;

                double simUseMinutes = 0;
                var useSw = Stopwatch.StartNew();

                for (int i = 0; i < items; i++)
                {
                    int minutes = _rng.Next(1, 4);
                    simUseMinutes += minutes;
                    Thread.Sleep(minutes * _simMinuteMs);
                }

                useSw.Stop();

                Result.UseTimeSimMinutes = simUseMinutes;
                Result.RealUseMs = useSw.ElapsedMilliseconds;
            }
        }
    }

    public class CustomerResult
    {
        public int CustomerId { get; set; }
        public int ItemCount { get; set; }
        public double WaitTimeSimMinutes { get; set; }
        public double UseTimeSimMinutes { get; set; }
        public long RealUseMs { get; set; }
    }
}
