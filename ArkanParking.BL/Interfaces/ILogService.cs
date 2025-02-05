using System;
using System.Transactions;

namespace ArkanParking.BL.Interfaces
{
    public interface ILogService : IDisposable
    {
        string LogPath { get; }
        void Write(string logInfo);
        string Read();
        void LogTransaction(Transaction p0);
    }
}