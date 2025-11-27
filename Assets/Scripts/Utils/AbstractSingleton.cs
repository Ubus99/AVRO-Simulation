public class AbstractSingleton<T> where T : new()
{
    static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new T();
            }
            return _instance;
        }
        set { _instance = value; }
    }
}
