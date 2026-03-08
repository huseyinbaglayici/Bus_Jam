namespace _Scripts.Core
{
    public interface ITransition
    {
        IState To { get; }
        IPredicate Condition { get; }
    }
}