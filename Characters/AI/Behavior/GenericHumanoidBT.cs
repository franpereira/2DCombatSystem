using UnityEngine;

namespace Konrad.Characters.AI.Behavior
{
    public class GenericHumanoidBT : BehaviorTree
    {
        [SerializeField] Character character;
        [SerializeField] float detectionRadius = 10f;

        protected override void Update()
        {
            if (character.enabled) base.Update();
        }
        protected override void Setup()
        {
            Root = new Selector();

            Sequence attackEnemy = new();
            CheckCharactersNearby ae1 = new(transform.position, detectionRadius, LayerMask.GetMask("Character"));
            FilterNearestEnemy ae2 = new(character);
            TaskWalkToTarget ae3 = new(character);
            TaskAttackTarget ae4 = new(character);
            attackEnemy.AttachAsChildren(new BehaviorNode[] { ae1, ae2, ae3, ae4 });

            Root.AttachAsChild(attackEnemy);
            Root.AttachAsChild(new TaskRandomWalking(character));
        }
    }
}