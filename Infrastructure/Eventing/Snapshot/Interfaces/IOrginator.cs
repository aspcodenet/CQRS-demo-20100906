namespace Eventing.Snapshot.Interfaces
{
    public interface IOrginator
    {
        void SetMemento(IMemento t);
        IMemento CreateMemento();
    }
}
