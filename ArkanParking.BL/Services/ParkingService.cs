using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Transactions;
using ArkanParking.BL.Interfaces;
using ArkanParking.BL.Models;

namespace ArkanParking.BL.Services
{
    public class ParkingService : IParkingService
    {
        private readonly List<Vehicle> vehicles;
        private decimal parkingBalance;
        private readonly int capacity;
        private readonly ILogService logService;
        private readonly ITimerService withdrawTimer;
        private readonly ITimerService logTimer;
        private bool timersStarted = false;

        public ParkingService(ITimerService withdrawTimer, ITimerService logTimer, ILogService logService)
        {
            this.withdrawTimer = withdrawTimer ?? throw new ArgumentNullException(nameof(withdrawTimer));
            this.logTimer = logTimer ?? throw new ArgumentNullException(nameof(logTimer));
            this.logService = logService ?? throw new ArgumentNullException(nameof(logService));
            this.capacity = Settings.ParkingCapacity;
            vehicles = new List<Vehicle>();
            parkingBalance = 0;
        }

        private void StartTimers()
        {
            if (!timersStarted)
            {
                withdrawTimer.Interval = 60000; 
                withdrawTimer.Elapsed += (sender, e) => ChargeVehicles();
                withdrawTimer.Start();

                
                logTimer.Interval = 60000; 
                logTimer.Elapsed += (sender, e) => logService.Write($"Поточний баланс парковки: {parkingBalance}");
                logTimer.Start();

                timersStarted = true;
                logService.Write("Таймери запущено");
            }
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
            
            if (vehicles.Count == 1)
            {
                StartTimers();
            }
            
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

        public void Dispose()
        {
            withdrawTimer?.Dispose();
            logTimer?.Dispose();
            logService?.Dispose();
        }
    }
}