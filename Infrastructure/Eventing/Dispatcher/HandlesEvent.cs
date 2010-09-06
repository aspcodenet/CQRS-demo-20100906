namespace Eventing.Dispatcher
{
    public interface IHandles
    {
    }
    public abstract class HandlesEvent<T> : IHandles
    {

        public abstract void Handle(T msg);


    }
}
