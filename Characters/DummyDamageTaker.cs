using Konrad.Damaging;
using UnityEngine;

namespace Konrad.Characters
{
    public class DummyDamageTaker : MonoBehaviour, IDamageTaker
    {
        public void TakeDamage(Damage damage)
        {
            Debug.Log($"{name} took {damage.Amount} damage from {damage.Source.name}");
        }
    }

}
