namespace _Scripts.Core

{
    public interface IState

    {
        void OnEnter();

        void Update();

        void OnExit();
    }
}