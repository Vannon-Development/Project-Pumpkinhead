using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

namespace Character
{
    [System.Serializable]
    public class ActionTree: ISerializationCallbackReceiver
    {
        [Serializable]
        private class SerializableActionNode
        {
            public AnimationClip animation;
            public ActionCommand command;
            public int childCount;

            public SerializableActionNode(AnimationClip animation, ActionCommand command, int childCount)
            {
                this.animation = animation;
                this.command = command;
                this.childCount = childCount;
            }
        }

        public Dictionary<ActionInitialState, List<ActionNode>> Branches =
            new Dictionary<ActionInitialState, List<ActionNode>>();

        [HideInInspector, SerializeField] private List<SerializableActionNode> nodes = new List<SerializableActionNode>();
        [HideInInspector, SerializeField] private int[] branchTypes;
        [HideInInspector, SerializeField] private int[] branchCounts;
        
        public void OnBeforeSerialize()
        {
            var branchStats = new List<(int, int)>();
            foreach (ActionInitialState state in Enum.GetValues(typeof(ActionInitialState)))
            {
                if (!Branches.ContainsKey(state)) continue;
                var branch = Branches[state];
                if (branch == null) continue;
                branchStats.Add(((int)state, branch.Count));
                foreach(var node in branch) WriteNode(node);
            }

            branchTypes = branchStats.Select((tuple, i) => tuple.Item1).ToArray();
            branchCounts = branchStats.Select((tuple, i) => tuple.Item2).ToArray();
        }

        public void OnAfterDeserialize()
        {
            var nodePos = 0;
            for (int index = 0; index < branchTypes.Length; index++)
            {
                for (int childNum = 0; childNum < branchCounts[index]; childNum++)
                {
                    List<ActionNode> branch = new List<ActionNode>();
                    nodePos = ReadNode(nodePos, branch);
                    Branches[(ActionInitialState)branchTypes[index]] = branch;
                }
            }
        }
        
        private void WriteNode(ActionNode node)
        {
            nodes.Add(new SerializableActionNode(node.Animation, node.Command, node.Children.Count));
            foreach(var child in node.Children)
                WriteNode(child);
        }

        private int ReadNode(int nodeIndex, List<ActionNode> branch)
        {
            var node = nodes[nodeIndex];
            var action = new ActionNode(node.animation, node.command);
            for (int counter = 0; counter < node.childCount; counter++)
            {
                nodeIndex += 1;
                ReadNode(nodeIndex, branch);
            }
            return nodeIndex + 1;
        }
    }

    public class ActionNode
    {
        public AnimationClip Animation;
        public ActionCommand Command;
        public List<ActionNode> Children;

        public ActionNode(AnimationClip clip, ActionCommand command)
        {
            Animation = clip;
            Command = command;
            Children = new List<ActionNode>();
        }
    }

    public enum ActionInitialState
    {
        Idle, 
        Walk, 
        Run, 
        Air
    }

    public enum ActionCommand
    {
        FastAttack,
        SlowAttack,
        SuperAttack,
        Dodge
    }
}