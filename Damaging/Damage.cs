using System.Collections.Generic;
using UnityEngine;

namespace Konrad.Damaging
{
    public struct Damage
    {
        public float Amount;
        public DamageType Type;
        public Vector2 Position;
        public GameObject Source;
        public List<DamageEffect> Effects;
        
        public Damage(float amount, Vector2 position, GameObject source, DamageType type, DamageEffect effect)
        {
            Amount = amount;
            Type = type;
            Position = position;
            Source = source;
            Effects = new List<DamageEffect> { effect };
        }
        
        public Damage(float amount, Vector2 position, GameObject source, DamageType type, List<DamageEffect> effects = null)
        {
            Amount = amount;
            Type = type;
            Position = position;
            Source = source;
            Effects = effects ?? new List<DamageEffect>();
        }

        public Vector2 DirectionTo(Vector2 targetPosition) => (Position - targetPosition).normalized;
    }
}