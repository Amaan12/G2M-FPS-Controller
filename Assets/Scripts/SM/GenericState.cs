public abstract class State<T>
{
    protected T owner;

    public State(T owner)
    {
        this.owner = owner;
    }

    public abstract void Enter();
    public abstract void Update();
    public abstract void Exit();
}