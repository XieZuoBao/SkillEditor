public abstract class Singleton<T> where T : new()
{
    private static T _instance;
    static object _lock = new object();

    [System.Diagnostics.DebuggerHidden]
    [System.Diagnostics.DebuggerStepThrough]
    public static T GetSingleton()
    {
        if (_instance == null)
        {
            lock (_lock)
            {
                if (_instance == null)
                    _instance = new T();
            }
        }
        return _instance;
    }
}