using UnityEngine;

namespace Konrad.Characters.AI.Behavior
{
    public abstract class BehaviorTree : MonoBehaviour
    {
        public BehaviorNode Root { get; protected set; }
        
        protected virtual void Start() => Setup();
        protected virtual void Update() => Root?.Evaluate();

        protected abstract void Setup();
    }
}