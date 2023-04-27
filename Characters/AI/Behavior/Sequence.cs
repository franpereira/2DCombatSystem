namespace Konrad.Characters.AI.Behavior
{
    /// <summary>
    /// All children will be executed in order.
    /// If any of them fails, the sequence is stopped and considered failed.
    /// </summary>
    public class Sequence : BehaviorNode
    {
        public override NodeState Evaluate()
        {
            BehaviorNode child = FirstChild;
            while (child != null)
            {
                switch (child.Evaluate())
                {
                    case NodeState.Success:
                        child = child.NextSibling;
                        continue;
                    case NodeState.Failure:
                        return NodeState.Failure;
                    case NodeState.Running:
                        return NodeState.Running;
                }
            }
            return NodeState.Success;
        }
    }
}