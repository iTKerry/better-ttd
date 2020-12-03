namespace BetterTTD.Coan.Pool
{
    public abstract class Poolable<T>
    {
        protected Poolable(T id)
        {
            Id = id;
        }

        public T Id { get; }
    }
}