namespace Konrad.Damaging
{
    public struct DamageEffect
    {
        public readonly EffectType Type;
        public readonly float Value;
        
        public DamageEffect(EffectType type, float value)
        {
            Type = type;
            Value = value;
        }
        
        public override string ToString()
        {
            return $"{Type}:{Value}";
        }
    }
    
    public enum EffectType
    {
        KnockBack,
        Stun
    }
    
    
}