public class StateMachine<T>
{
    private State<T> currentState;

    public void ChangeState(State<T> newState)
    {
        currentState?.Exit();
        currentState = newState;
        currentState?.Enter();
    }

    public void Update()
    {
        currentState?.Update();
    }
}