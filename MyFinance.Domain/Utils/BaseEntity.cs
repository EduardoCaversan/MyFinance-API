namespace MyFinance.Domain.Utils
{
    public abstract class BaseEntity
    {
        public bool Removed { get; set; }

        public virtual void Remove()
        {
            Removed = true;
        }
    }
}