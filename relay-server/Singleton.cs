namespace relay_server;

public class Singleton<T> where T : Singleton<T>
{
    public static T? Instance;

    public Singleton()
    {
        if (Instance == null)
        {
            Instance = (T)this;
        }
    }
}