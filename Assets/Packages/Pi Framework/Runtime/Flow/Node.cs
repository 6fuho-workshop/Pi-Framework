using UnityEngine;

namespace PF.FlowNode
{
    public class FSM
    {
        // Placeholder for FSM implementation
    }

    public class FSMConnection
    {
        // Placeholder for FSMConnection implementation
    }



    ///<summary>Implement this interface in any MonoBehaviour attached on FSMOwner gameobject to get relevant state callbacks</summary>
    public interface IStateCallbackReceiver
    {
        ///<summary>Called when a state enters</summary>
		void OnStateEnter(IState state);
        ///<summary>Called when a state updates</summary>
        void OnStateUpdate(IState state);
        ///<summary>Called when a state exists</summary>
        void OnStateExit(IState state);
    }

    /// <summary>
    /// Base class for all nodes in the flow node system.
    /// </summary>
    public abstract class Node
    {
        /// <summary>
        /// Gets the name of the node.
        /// </summary>
        public abstract string NodeName { get; }
        /// <summary>
        /// Gets the description of the node.
        /// </summary>
        public abstract string NodeDescription { get; }
        /// <summary>
        /// Initializes the node.
        /// </summary>
        public abstract void Initialize();
    }

    public class FSMNode : Node
    {
        public override string NodeName => "FSM Node";
        public override string NodeDescription => "Base class for FSM nodes that live within an FSM Graph.";
        public override void Initialize()
        {
            // Initialization logic for FSMNode
        }
    }

    public class BTNode : Node
    {
        public override string NodeName => "BT Node";
        public override string NodeDescription => "Super Base class for BehaviourTree nodes that can live within a BehaviourTree Graph.";
        public override void Initialize()
        {
            // Initialization logic for BTNode
        }
    }
}
