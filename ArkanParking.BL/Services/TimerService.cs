using System;
using System.Timers;
using ArkanParking.BL.Interfaces;

// TODO:створи клас TimerService, що реалізує інтерфейс ITimerService.
// Сервіс повинен бути лише обгорткою над таймерами системи (System.Timers).




namespace ArkanParking.BL.Services
{
    public class TimerService : ITimerService, IDisposable
    {
        private readonly Timer timer;

        public event ElapsedEventHandler Elapsed;

        public double Interval
        {
            get => timer.Interval;
            set => timer.Interval = value;
        }

        public TimerService()
        {
            timer = new Timer();
            timer.AutoReset = true;
            timer.Elapsed += OnElapsed;
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            Elapsed?.Invoke(sender, e);
        }

        public void Start()
        {
            timer.Start();
        }

        public void Stop()
        {
            timer.Stop();
        }

        public void Start(Action action, int interval)
        {
            Interval = interval;
            timer.Elapsed += (sender, args) => action();
            timer.Start();
        }

        public void Dispose()
        {
            timer.Dispose();
        }
    }
}