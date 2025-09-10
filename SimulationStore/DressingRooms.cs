using System;
using System.Threading;

namespace IN451_Unit6_Justin_Meyer
{
    public sealed class DressingRooms
    {
        private readonly SemaphoreSlim _semaphore;
        public int RoomCount { get; }

        public DressingRooms() : this(3) { }

        public DressingRooms(int rooms)
        {
            if (rooms <= 0) throw new ArgumentOutOfRangeException(nameof(rooms), "Rooms must be > 0.");
            RoomCount = rooms;
            _semaphore = new SemaphoreSlim(rooms, rooms);
        }

        public IDisposable RequestRoom()
        {
            _semaphore.Wait();
            return new Releaser(_semaphore);
        }

        private sealed class Releaser : IDisposable
        {
            private SemaphoreSlim _sem;
            private bool _disposed;

            public Releaser(SemaphoreSlim sem) => _sem = sem;

            public void Dispose()
            {
                if (_disposed) return;
                _disposed = true;
                _sem.Release();
            }
        }
    }
}
