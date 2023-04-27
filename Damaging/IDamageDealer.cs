
namespace Konrad.Damaging
{
    public interface IDamageDealer
    {
        /// <summary>
        /// Deal damage to all targets on an area.
        /// </summary>
        public void DealDamage();
        
        /// <summary>
        /// Deal damage to just a single target.
        /// </summary>
        /// <param name="damageTaker"></param>
        public void DealDamage(IDamageTaker damageTaker);
    }
}