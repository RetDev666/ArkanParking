using System;
using System.Timers;
using ArkanParking.BL.Interfaces;

namespace ArkanParking.BL.Services 
{
    public class TimerService : ITimerService, IDisposable
    {
        private readonly Timer _timer;
        private Action _currentAction;
        private bool _isDisposed;

        public event ElapsedEventHandler Elapsed;

        public double Interval
        {
            get => _timer.Interval;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("Інтервал має бути більше нуля.", nameof(value));
                _timer.Interval = value;
            }
        }

        public bool IsActive => _timer.Enabled;  

        public TimerService()
        {
            _timer = new Timer();
            _timer.AutoReset = true;
            _timer.Elapsed += OnElapsed;
            _isDisposed = false;
        }

        private void OnElapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                _currentAction?.Invoke();
                Elapsed?.Invoke(sender, e);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка зворотного виклику таймера: {ex.Message}");
            }
        }

        public void Start()
        {
            ThrowIfDisposed();
            if (_timer.Enabled)
            {
                Console.WriteLine("Таймер уже працює.");
                return;
            }
            _timer.Start();
            Console.WriteLine($"Таймер запустився о {DateTime.Now}. Інтервал: {Interval}мс");
        }

        public void Stop()
        {
            ThrowIfDisposed();
            _timer.Stop();
            Console.WriteLine($"Таймер зупинився на {DateTime.Now}");
        }

        public void Start(Action action, int interval)
        {
            ThrowIfDisposed();
            
            if (interval <= 0)
                throw new ArgumentException("Інтервал має бути більше нуля.", nameof(interval));

            _currentAction = action ?? throw new ArgumentNullException(nameof(action));
            Interval = interval;
            
            Start();
        }

        public void StartCharging(Action chargeAction)
        {
            ThrowIfDisposed();

            if (chargeAction == null)
                throw new ArgumentNullException(nameof(chargeAction));

            Stop();

            const int chargingInterval = 60000; 
            _currentAction = () =>
            {
                try
                {
                    chargeAction();
                    Console.WriteLine($"Дія зарядки виконана о {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Помилка заряджання: {ex.Message}");
                }
            };

            Start(_currentAction, chargingInterval);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Stop();
                    _timer.Elapsed -= OnElapsed;
                    _timer.Dispose();
                }
                _isDisposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void ThrowIfDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException(nameof(TimerService));
            }
        }
    }
}