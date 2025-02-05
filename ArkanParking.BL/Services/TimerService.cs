using System;
using System.Timers;
using ArkanParking.BL.Interfaces;
using ArkanParking.BL.Models;
using Timer = System.Threading.Timer;

namespace ArkanParking.BL.Services;
// TODO:створи клас TimerService, що реалізує інтерфейс ITimerService.
// Сервіс повинен бути лише обгорткою над таймерами системи (System.Timers).

public class TimerService : ITimerService
{
    private readonly Timer _timer;

    public event ElapsedEventHandler Elapsed
    {
        add => _timer.Elapsed += value;
        remove => _timer.Elapsed -= value;
    }

    public double Interval
    {
        get => _timer.Interval;
        set => _timer.Interval = value;
    }

    public TimerService(double interval)
    {
        _timer = new Timer(interval);
        _timer.AutoReset = true; // Таймер автоматично повторюється
    }

    public void Start()
    {
        _timer.Start();
    }

    public void Stop()
    {
        _timer.Stop();
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}