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
        private readonly List<Vehicle> vehicles;
        private decimal parkingBalance;
        private readonly int capacity;
        private readonly ILogService logService;
        private readonly ITimerService withdrawTimer;
        private readonly ITimerService logTimer;

        public ParkingService(ITimerService withdrawTimer, ITimerService logTimer, ILogService logService)
        {
            this.withdrawTimer = withdrawTimer;
            this.logTimer = logTimer;
            this.logService = logService;
            this.capacity = Settings.ParkingCapacity;
            vehicles = new List<Vehicle>();
            parkingBalance = 0;
        }

        public decimal GetBalance() => parkingBalance;

        public int GetCapacity() => capacity;

        public int GetFreePlaces() => capacity - vehicles.Count;

        public ReadOnlyCollection<Vehicle> GetVehicles() => vehicles.AsReadOnly();

        public void AddVehicle(Vehicle vehicle)
        {
            if (vehicles.Count >= capacity)
            {
                throw new InvalidOperationException("Парковка заповнена!");
            }
            if (vehicles.Any(v => v.Id == vehicle.Id))
            {
                throw new ArgumentException("Транспортний засіб з таким ID вже існує.");
            }
            vehicles.Add(vehicle);
            logService.Write($"Додано транспортний засіб: {vehicle.Id}");
        }

        public void RemoveVehicle(string vehicleId)
        {
            var vehicle = vehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (vehicle == null)
            {
                throw new ArgumentException("Транспортний засіб незнайдено!");
            }
            if (vehicle.Balance < 0)
            {
                throw new InvalidOperationException("Неможливо видалити транспортний засіб з негативним балансом.");
            }
            vehicles.Remove(vehicle);
            logService.Write($"Видалено транспортний засіб: {vehicle.Id}");
        }

        public void TopUpVehicle(string vehicleId, decimal amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Сума поповнення повинна бути позитивною.");
            }
            var vehicle = vehicles.FirstOrDefault(v => v.Id == vehicleId);
            if (vehicle == null)
            {
                throw new ArgumentException("Транспортний засіб незнайдено!");
            }
            vehicle.AddBalance(amount);
            logService.Write($"Поповнено баланс {vehicle.Id} на {amount} у.о.");
        }

        public void ChargeVehicles()
        {
            foreach (var vehicle in vehicles)
            {
                decimal fee = Settings.GetFee(vehicle.VehicleType);
                if (vehicle.Balance >= fee)
                {
                    vehicle.DeductBalance(fee);
                    parkingBalance += fee;
                    logService.Write($"Списано {fee} у.о. з {vehicle.Id}. Залишок балансу: {vehicle.Balance}");
                }
                else
                {
                    decimal deficit = fee - vehicle.Balance;
                    decimal penalty = deficit * Settings.PenaltyCoefficient;
                    vehicle.DeductBalance(fee + penalty);
                    parkingBalance += fee + penalty;
                    logService.Write($"Списано {fee + penalty} у.о. (з урахуванням штрафу) з {vehicle.Id}. Баланс став негативним.");
                }
            }
        }

        public TransactionInfo[] GetLastParkingTransactions()
        {
            return Array.Empty<TransactionInfo>();
        }

        public string ReadFromLog() => logService.Read();

        public void Dispose() => logService.Dispose();
    }