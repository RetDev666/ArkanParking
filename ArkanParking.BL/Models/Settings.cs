using System;
using System.Collections.Generic;

namespace ArkanParking.BL.Models;
// TODO:створи клас Settings.
// Деталі реалізації залишаються на твій розсуд, вони повинні лише відповідати вимогам домашнього завдання.

public static class Settings
{
    public static int Capacity { get; } = 10;
    public static double InitialParkingBalance { get; } = 0;
    public static int FeePeriodInSeconds { get; } = 5;
    public static int LogPeriodInSeconds { get; } = 60;
    public static double PenaltyCoefficient { get; } = 2.5;

    public static readonly Dictionary<VehicleType, double> Tariffs = new()
    {
        { VehicleType.PassengerCar, 2 },
        { VehicleType.Truck, 5 },
        { VehicleType.Bus, 3.5 },
        { VehicleType.Motorcycle, 1 }
    };
}