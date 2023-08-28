namespace Endsley
{
    public interface IDamageSource
    {
        void DealDamageTo(IDamageable target, int amount);
    }
}
