using System.IO;
using ArkanParking.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Transactions;

namespace ArkanParking.BL.Services;
// TODO: створи клас LogService, що реалізує інтерфейс ILogService.
// Є одна явна вимога — для методу читання, якщо файл не знайдено, потрібно викинути InvalidOperationException.
// Інші деталі реалізації залишаються на твій розсуд, але вони повинні відповідати вимогам інтерфейсу 
// та тестам. Наприклад, у LogServiceTests можна знайти необхідний формат конструктора.

public class LogService : ILogService
{
    private readonly string _logFilePath = "Transactions.log";
    private List<Transaction> _currentTransactions;
    private ILogService _logServiceImplementation;

    public LogService(string logFilePath)
    {
        _currentTransactions = new List<Transaction>();
    }

    public void LogTransaction(Transaction transaction)
    {
        _currentTransactions.Add(transaction);
    }

    public void WriteLogsToFile()
    {
        using StreamWriter writer = new StreamWriter(_logFilePath, append: true);
        foreach (var transaction in _currentTransactions)
        {
            writer.WriteLine(transaction.ToString());
        }
        _currentTransactions.Clear();
    }

    public IEnumerable<string> ReadTransactionHistory()
    {
        if (!File.Exists(_logFilePath))
            return new List<string>();

        return File.ReadAllLines(_logFilePath);
    }

    public void Dispose()
    {
        _logServiceImplementation.Dispose();
    }

    public string LogPath => _logServiceImplementation.LogPath;

    public void Write(string logInfo)
    {
        _logServiceImplementation.Write(logInfo);
    }

    public string Read()
    {
        return _logServiceImplementation.Read();
    }
}