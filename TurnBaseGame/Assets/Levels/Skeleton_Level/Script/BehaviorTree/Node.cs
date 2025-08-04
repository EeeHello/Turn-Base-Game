using System.Collections.Generic;

namespace BehaviorTree
{
    public enum NodeState
    {
        RUNNING,
        SUCCESS,
        FAILURE
    }

    public class Node
    {
        protected NodeState state;

        public Node parent;
        protected List<Node> children = new List<Node>();

        public Node()
        {
            parent = null;
        }

        public Node(List<Node> children)
        {
            foreach (Node child in children)
            {
                AddChild(child);
            }
        }

        public virtual NodeState Evaluate() => NodeState.FAILURE;

        protected void AddChild(Node child)
        {
            child.parent = this;
            children.Add(child);
        }
    }
}
