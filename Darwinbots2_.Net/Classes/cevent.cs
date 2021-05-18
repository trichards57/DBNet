public class cevent
{
    // event definition for console input
    // I really can't remember why I have defined
    // an event for it
    public delegate void textenteredHandler(int ind, string text);

    public event textenteredHandler eventtextentered;

    public void fire(int a, string t)
    {
        eventtextentered?.Invoke(a, t);
    }
}
