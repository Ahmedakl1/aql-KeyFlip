using System;
using System.Threading;
using aql.KeyFlip.Core.Common;

namespace aql.KeyFlip.Windows.SingleInstance;

public sealed class SingleInstanceMutex : IDisposable
{
    private Mutex? _mutex;
    private bool _hasHandle;

    public bool IsOnlyInstance { get; private set; }

    public SingleInstanceMutex(string? mutexName = null)
    {
        string name = mutexName ?? AppConstants.SingleInstanceMutexName;
        try
        {
            _mutex = new Mutex(true, name, out bool createdNew);
            _hasHandle = createdNew;
            IsOnlyInstance = createdNew;
        }
        catch
        {
            IsOnlyInstance = false;
        }
    }

    public void Dispose()
    {
        if (_hasHandle && _mutex != null)
        {
            try
            {
                _mutex.ReleaseMutex();
            }
            catch { }
            _hasHandle = false;
        }
        _mutex?.Dispose();
        _mutex = null;
    }
}
