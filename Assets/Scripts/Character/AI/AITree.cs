using System.Collections.Generic;
using UnityEngine;

namespace Character.AI
{
    public class Tree 
    {
        
    }
    
    internal enum Status { Success, Failure, Running }

    internal interface INode
    {
        public Status Tick();
    }

    internal abstract class CompositeNode : INode
    {
        protected List<INode> children;
        public abstract Status Tick();
    }

    internal abstract class DecoratorNode : INode
    {
        protected INode child;
        public abstract Status Tick();
    }

    internal abstract class LeafNode : INode
    {
        public abstract Status Tick();
    }

    internal class SequenceNode : CompositeNode
    {
        public override Status Tick()
        {
            foreach (var n in children)
            {
                var result = n.Tick();
                if (result != Status.Success)
                    return result;
            }

            return Status.Success;
        }
    }

    internal class FallbackNode : CompositeNode
    {
        public override Status Tick()
        {
            foreach (var n in children)
            {
                var result = n.Tick();
                if (result != Status.Failure)
                    return result;
            }

            return Status.Failure;
        }
    }

    internal class InverterNode : DecoratorNode
    {
        public override Status Tick()
        {
            var result = child.Tick();
            if (result == Status.Success) return Status.Failure;
            if (result == Status.Failure) return Status.Success;
            return result;
        }
    }
}
