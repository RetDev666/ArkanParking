using ArkanParking.BL.Interfaces;
using System.Timers;


namespace ArkanParking.BL.Tests
{
    public class FakeTimerService :  ITimerService
    {
        public bool IsActive { get; }
        public double Interval { get; set; }

        public event ElapsedEventHandler Elapsed;

        public void FireElapsedEvent()
        {
            Elapsed?.Invoke(this, null);
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public void Dispose()
        {
        }

    }
}
