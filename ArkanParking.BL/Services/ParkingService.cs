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

public class ParkingService : IParkingService
    {
        private List<Vehicle> _vehicles;
        private decimal _parkingBalance;
        private readonly ILogService _logService;
        private IParkingService _parkingServiceImplementation;

        public ParkingService(ILogService logService)
        {
            _vehicles = new List<Vehicle>();
            _parkingBalance = 0;
            _logService = logService;
        }

        public bool ParkVehicle(Vehicle vehicle)
        {
            if (_vehicles.Count >= Settings.Capacity)
                throw new InvalidOperationException("Паркінг переповнений.");

            _vehicles.Add(vehicle);
            return true;
        }

        public decimal GetBalance() => _parkingBalance;

        public int GetCapacity() => Settings.Capacity;

        public int GetFreePlaces() => Settings.Capacity - _vehicles.Count;

        public ReadOnlyCollection<Vehicle> GetVehicles() => _vehicles.AsReadOnly();
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
            var vehicle = _vehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (vehicle == null)
                throw new InvalidOperationException("Транспортний засіб не знайдено.");

            vehicle.TopUpBalance(sum);
        }

        public TransactionInfo[] GetLastParkingTransactions()
        {
            return _parkingServiceImplementation.GetLastParkingTransactions();
        }

        public bool RemoveVehicle(string vehicleId)
        {
            var vehicle = _vehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (vehicle == null)
                throw new InvalidOperationException("Транспортний засіб не знайдено.");

            if (vehicle.Balance < 0)
                throw new InvalidOperationException("Не можна видалити транспортний засіб із боргом.");

            _vehicles.Remove(vehicle);
            return true;
        }

        public void DeductParkingFee()
        {
            foreach (var vehicle in _vehicles)
            {
                decimal fee = Settings.Tariffs[vehicle.Type];
                if (vehicle.Balance >= fee)
                {
                    vehicle.Deduct(fee);
                    _parkingBalance += fee;
                    _logService.LogTransaction(new TransactionInfo(vehicle.Id, fee));
                }
                else
                {
                    decimal penaltyFee = fee + (fee - vehicle.Balance) * (decimal)Settings.PenaltyCoefficient;
                    vehicle.Deduct(penaltyFee);
                    _parkingBalance += penaltyFee;
                    _logService.LogTransaction(new TransactionInfo(vehicle.Id, penaltyFee));
                }
            }
        }

        public string ReadFromLog() => _logService.ReadLog();
        public void Dispose()
        {
            _parkingServiceImplementation.Dispose();
        }
    }