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
    private readonly Timer _deductTimer;
    private readonly Timer _logTimer;
    private readonly Action _deductAction;
    private readonly Action _logAction;
    private ITimerService _timerServiceImplementation;

    public TimerService(Action deductAction, Action logAction)
    {
        _deductAction = deductAction;
        _logAction = logAction;

        // Таймер для списання коштів
        _deductTimer = new Timer(Settings.FeePeriodInSeconds * 1000);
        _deductTimer.Elapsed += (sender, args) => _deductAction();

        // Таймер для запису логів
        _logTimer = new Timer(Settings.LogPeriodInSeconds * 1000);
        _logTimer.Elapsed += (sender, args) => _logAction();
    }

    public event ElapsedEventHandler Elapsed
    {
        add => _timerServiceImplementation.Elapsed += value;
        remove => _timerServiceImplementation.Elapsed -= value;
    }

    public double Interval
    {
        get => _timerServiceImplementation.Interval;
        set => _timerServiceImplementation.Interval = value;
    }

    public void Start()
    {
        _deductTimer.Start();
        _logTimer.Start();
    }

    public void Stop()
    {
        _deductTimer.Stop();
        _logTimer.Stop();
    }

    public void Dispose()
    {
        _deductTimer.Dispose();
    }
}