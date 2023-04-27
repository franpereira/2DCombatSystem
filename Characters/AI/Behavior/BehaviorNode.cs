using System;
using System.Collections.Generic;

namespace Konrad.Characters.AI.Behavior
{
    public abstract class BehaviorNode
    {
        public NodeState State { get; protected set; }
        public BehaviorNode Parent { get; set; }
        public BehaviorNode FirstChild { get; set; }
        public BehaviorNode NextSibling { get; set; }

        readonly Dictionary<string, object> _data = new();

        public abstract NodeState Evaluate();

        #region Data
        public object GetData(string key)
        {
            if (_data.TryGetValue(key, out object value))
                return value;
            return Parent?.GetData(key);
        }
        
        public bool HasData(string key)
        {
            if (_data.ContainsKey(key))
                return true;
            BehaviorNode node = Parent;
            while (node != null)
            {
                if (node.HasData(key))
                    return true;
                node = node.Parent;
            }
            return false;
        }
        
        public void SetData(string key, object value)
        {
            _data[key] = value;
            Parent?.SetData(key, value);
        }

        public bool RemoveData(string key)
        {
            bool existed = false;
            if (_data.ContainsKey(key))
            {
                _data.Remove(key);
                existed = true;
            }
            BehaviorNode node = Parent;
            while (node != null)
            {
                if (node.RemoveData(key))
                    existed = true;
                node = node.Parent;
            }
            return existed;
        }
        #endregion Data

        #region Attachers
        public void AttachAsFirstChild(BehaviorNode node)
        {
            node.Parent = this;
            BehaviorNode aux = FirstChild;
            FirstChild = node;
            if (aux != null) node.NextSibling = aux;
        }
        
        public void AttachAsChild(BehaviorNode node)
        {
            node.Parent = this;
            if (FirstChild == null) FirstChild = node;
            else FirstChild.AttachAsSibling(node);
        }

        public void AttachAsChildren(IEnumerable<BehaviorNode> nodes)
        {
            foreach (var node in nodes) AttachAsChild(node);
        }

        public void AttachAsFirstSibling(BehaviorNode node)
        {
            throw new NotImplementedException("AttachAsFirstSibling");
        }
        public void AttachAsSibling(BehaviorNode node)
        {
            node.Parent = Parent;
            if (NextSibling == null) NextSibling = node;
            else NextSibling.AttachAsSibling(node);
        }
        #endregion Attachers

        public void AttachAsSiblings(IEnumerable<BehaviorNode> nodes)
        {
            foreach (var node in nodes) AttachAsSibling(node);
        }

    }

    public enum NodeState
    {
        Success,
        Failure,
        Running
    }
}