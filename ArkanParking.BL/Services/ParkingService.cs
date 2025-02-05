using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Transactions;
using ArkanParking.BL.Interfaces;
using ArkanParking.BL.Models;

namespace ArkanParking.BL.Services;
// TODO: створи клас ParkingService, що реалізує інтерфейс IParkingService.
// Якщо спробувати додати транспортний засіб на заповнену парковку, повинно бути викинуто InvalidOperationException.
// Якщо спробувати видалити транспортний засіб із негативним балансом (боргом), також повинно бути викинуто InvalidOperationException.
// Інші правила валідації та формат конструктора вказані в тестах.
// Інші деталі реалізації залишаються на твій розсуд, але вони повинні відповідати вимогам інтерфейсу 
// та тестам. Наприклад, у ParkingServiceTests можна знайти необхідний формат конструктора та правила валідації.

public class ParkingService<FakeTimerService> : IParkingService
{
    private List<Vehicle> _vehicles;
    private double _parkingBalance;
    private ILogService _logService;
    private IParkingService _parkingServiceImplementation;

    public ParkingService(ILogService logService)
    {
        _vehicles = new List<Vehicle>();
        _parkingBalance = 0;
        _logService = logService;
    }

    public ParkingService(FakeTimerService withdrawTimer, FakeTimerService logTimer, ILogService logService)
    {
        throw new NotImplementedException();
    }

    public bool ParkVehicle(Vehicle vehicle)
    {
        if (_vehicles.Count >= Settings.Capacity)
            return false; // Паркінг заповнений

        _vehicles.Add(vehicle);
        return true;
    }

    public decimal GetBalance()
    {
        return _parkingServiceImplementation.GetBalance();
    }

    public int GetCapacity()
    {
        return _parkingServiceImplementation.GetCapacity();
    }

    public int GetFreePlaces()
    {
        return _parkingServiceImplementation.GetFreePlaces();
    }

    public ReadOnlyCollection<Vehicle> GetVehicles()
    {
        return _parkingServiceImplementation.GetVehicles();
    }

    public void AddVehicle(Vehicle vehicle)
    {
        _parkingServiceImplementation.AddVehicle(vehicle);
    }

    void IParkingService.RemoveVehicle(string vehicleId)
    {
        _parkingServiceImplementation.RemoveVehicle(vehicleId);
    }

    public void TopUpVehicle(string vehicleId, decimal sum)
    {
        _parkingServiceImplementation.TopUpVehicle(vehicleId, sum);
    }

    public TransactionInfo[] GetLastParkingTransactions()
    {
        return _parkingServiceImplementation.GetLastParkingTransactions();
    }

    public string ReadFromLog()
    {
        return _parkingServiceImplementation.ReadFromLog();
    }

    public bool RemoveVehicle(string vehicleId)
    {
        var vehicle = _vehicles.FirstOrDefault(v => v.Id == vehicleId);
        if (vehicle == null) return false;

        _vehicles.Remove(vehicle);
        return true;
    }

    public void DeductParkingFee()
    {
        foreach (var vehicle in _vehicles)
        {
            double fee = Settings.Tariffs[vehicle.Type];
            if (vehicle.Balance >= fee)
            {
                vehicle.Deduct(fee);
                _parkingBalance += fee;
                _logService.LogTransaction(new Transaction(vehicle.Id, fee));
            }
            else
            {
                // Розрахунок штрафу
                double penaltyFee = fee + (fee - vehicle.Balance) * Settings.PenaltyCoefficient;
                vehicle.Deduct(penaltyFee);
                _parkingBalance += penaltyFee;
                _logService.LogTransaction(new Transaction(vehicle.Id, penaltyFee));
            }
        }
    }

    public double GetParkingBalance() => _parkingBalance;
    public int GetAvailableSpaces() => Settings.Capacity - _vehicles.Count;
    public void Dispose()
    {
        _parkingServiceImplementation.Dispose();
    }
}