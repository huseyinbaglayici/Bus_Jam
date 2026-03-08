using Cysharp.Threading.Tasks;

namespace _Scripts.Runtime.Interfaces
{
    public interface ICommandAsync<T>
    {
        UniTask ExecuteAsync(T parameter);
    }

    public interface ICommand
    {
        void Execute();
    }
}