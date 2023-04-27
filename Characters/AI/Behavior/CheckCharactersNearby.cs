using System.Collections.Generic;
using UnityEngine;

namespace Konrad.Characters.AI.Behavior
{
    public class CheckCharactersNearby : BehaviorNode
    {
        readonly Vector2 _center;
        readonly float _radius;
        readonly LayerMask _layerMask;

        readonly Collider2D[] _colliders = new Collider2D[10];
        
        public CheckCharactersNearby(Vector2 center, float radius, LayerMask characterLayerMask)
        {
            _center = center;
            _radius = radius;
            _layerMask = characterLayerMask;
        }

        
        public override NodeState Evaluate()
        {
            const string dataKey = "charactersNearby";
            
            int count = Physics2D.OverlapCircleNonAlloc(_center, _radius, _colliders, _layerMask);
            if (count == 0) return NodeState.Failure;

            List<Character> list = new(_colliders.Length);
            foreach (var collider in _colliders)
            {
                if (ReferenceEquals(collider, null)) continue;
                bool isCharacter = collider.gameObject.TryGetComponent(out Character ch);
                if (isCharacter) list.Add(ch);
            }
            
            SetData(dataKey, list);
            return NodeState.Success;
        }
    }
}