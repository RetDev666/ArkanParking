using System.IO;
using ArkanParking.BL.Interfaces;
using System;
using System.Collections.Generic;
using System.Transactions;
using ArkanParking.BL.Models;

namespace ArkanParking.BL.Services;
// TODO: створи клас LogService, що реалізує інтерфейс ILogService.
// Є одна явна вимога — для методу читання, якщо файл не знайдено, потрібно викинути InvalidOperationException.
// Інші деталі реалізації залишаються на твій розсуд, але вони повинні відповідати вимогам інтерфейсу 
// та тестам. Наприклад, у LogServiceTests можна знайти необхідний формат конструктора.

public class LogService : ILogService
{
    public string LogPath { get; }
    private readonly StreamWriter streamWriter;
    private ILogService _logServiceImplementation;

    public LogService(string logFilePath)
    {
        LogPath = logFilePath;
        streamWriter = new StreamWriter(new FileStream(logFilePath, FileMode.Append, FileAccess.Write));
    }

    public void Write(string logMessage)
    {
        streamWriter.WriteLine($"{DateTime.Now}: {logMessage}");
        streamWriter.Flush();
    }

    public string Read()
    {
        if (!File.Exists(LogPath))
        {
            throw new FileNotFoundException("Лог-файл не знайдено");
        }
        return File.ReadAllText(LogPath);
    }

    public void LogTransaction(Transaction p0)
    {
        _logServiceImplementation.LogTransaction(p0);
    }

    public string ReadLog()
    {
        return _logServiceImplementation.ReadLog();
    }

    public void LogTransaction(TransactionInfo p0)
    {
        _logServiceImplementation.LogTransaction(p0);
    }

    public void Dispose()
    {
        streamWriter.Dispose();
    }
}