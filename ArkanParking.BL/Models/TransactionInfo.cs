using System;

namespace ArkanParking.BL.Models;
// TODO: створи структуру TransactionInfo.
// Обов'язково реалізуй властивість Sum (типу decimal) — використовується у тестах.
// Інші деталі реалізації залишаються на твій розсуд, вони повинні лише відповідати вимогам домашнього завдання.

public class TransactionInfo
{
    public DateTime Timestamp { get; }
    public string VehicleId { get; }
    public decimal Sum { get; }

    public TransactionInfo(string vehicleId, decimal amount)
    {
        Timestamp = DateTime.Now;
        VehicleId = vehicleId;
        Sum = amount;
    }

    public override string ToString()
    {
        return $"{Timestamp}: Vehicle {VehicleId}, Amount: {Sum}";
    }
}