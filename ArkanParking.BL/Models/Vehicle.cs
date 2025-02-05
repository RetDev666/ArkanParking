using System;
using System.Text.RegularExpressions;

namespace ArkanParking.BL.Models;
// TODO: створи клас Vehicle.
// Властивості: Id (типу string), VehicleType (типу VehicleType), Balance (типу decimal).
// Формат ідентифікатора описаний у завданні.
// Id та VehicleType не повинні змінюватись після встановлення значень.
// Властивість Balance має змінюватись лише в проєкті CoolParking.BL.
// Тип конструктора показаний у тестах, і він повинен мати валідацію, яка також зрозуміла з тестів.
// Статичний метод GenerateRandomRegistrationPlateNumber повинен повертати випадково згенерований унікальний ідентифікатор.

public class Vehicle
{
    public string Id { get; private set; }
    public VehicleType Type { get; private set; }
    public decimal Balance { get; private set; }

    public Vehicle(string id, VehicleType type, decimal initialBalance)
    {
        if (!IsValidId(id))
            throw new ArgumentException("Invalid vehicle ID format.");

        Id = id;
        Type = type;
        Balance = initialBalance;
    }

    private bool IsValidId(string id)
    {
        return System.Text.RegularExpressions.Regex.IsMatch(id, "^[A-Z]{2}-\\d{4}-[A-Z]{2}$");
    }

    public void TopUpBalance(decimal amount)
    {
        if (amount <= 0) throw new ArgumentException("Amount must be positive.");
        Balance += amount;
    }

    public void Deduct(decimal amount)
    {
        Balance -= amount;
    }
}
