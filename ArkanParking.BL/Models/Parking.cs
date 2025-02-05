using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ArkanParking.BL.Interfaces;
using ArkanParking.BL.Services;

namespace ArkanParking.BL.Models;
// TODO: створи клас Parking.
// Деталі реалізації залишаються на твій розсуд, вони повинні лише відповідати вимогам
// завдання та бути узгодженими з іншими класами та тестами.

public class Parking
{
    public double Balance { get; private set; }
    public List<Vehicle> Vehicles { get; private set; }

    public Parking()
    {
        Balance = Settings.InitialParkingBalance;
        Vehicles = new List<Vehicle>();
    }

    public void AddToBalance(double amount)
    {
        Balance += amount;
    }

    public void DeductFromBalance(double amount)
    {
        Balance -= amount;
    }

    public void AddVehicle(Vehicle vehicle)
    {
        Vehicles.Add(vehicle);
    }

    public void RemoveVehicle(string vehicleId)
    {
        var vehicle = Vehicles.Find(v => v.Id == vehicleId);
        if (vehicle != null)
        {
            Vehicles.Remove(vehicle);
        }
    }
}