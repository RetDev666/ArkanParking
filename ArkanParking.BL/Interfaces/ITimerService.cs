using System;
using System.Timers;

namespace ArkanParking.BL.Interfaces
{
    public interface ITimerService : IDisposable
    {
        event ElapsedEventHandler Elapsed;
        double Interval { get; set; }
        void Start();
        void Stop();
    }
}
