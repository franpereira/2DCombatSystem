namespace Konrad.Characters.AI.Behavior
{
    public class Selector : BehaviorNode
    {

        public override NodeState Evaluate()
        {
            BehaviorNode child = FirstChild;
            while (child != null)
            {
                switch (child.Evaluate())
                {
                    case NodeState.Failure:
                        child = child.NextSibling;
                        continue;
                    case NodeState.Success:
                        return NodeState.Success;
                    case NodeState.Running:
                        return NodeState.Success;
                }
            }
            return NodeState.Failure;
        }
    }
}