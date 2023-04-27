using System.Collections.Generic;
using UnityEngine;

namespace Konrad.Characters.AI.Behavior
{
    public class FilterNearestEnemy : BehaviorNode
    {
        readonly Character _thisCharacter;

        public FilterNearestEnemy(Character character) => _thisCharacter = character;

        public override NodeState Evaluate()
        {
            const string targetKey = "targetCharacter";
            const string nearbyKey = "charactersNearby";

            var nearbyList = GetData(nearbyKey) as List<Character>;
            if (nearbyList == null) return NodeState.Failure;
            Character nearest = null;
            float smallest = float.MaxValue;
            foreach (var character in nearbyList)
            {
                if (character.IsAlive == false || character == _thisCharacter) continue;

                var thisPos = _thisCharacter.transform.position;
                var enemyPos = character.transform.position;
                float distance = Vector2.Distance(thisPos, enemyPos);
                if (distance < smallest)
                {
                    smallest = distance;
                    nearest = character;
                }
            }
            if (nearest == null) return NodeState.Failure;
            SetData(targetKey, nearest);
            return NodeState.Success;
        }
    }
}