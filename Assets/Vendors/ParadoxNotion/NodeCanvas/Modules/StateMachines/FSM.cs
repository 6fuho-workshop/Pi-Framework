using System.Linq;
using System.Collections.Generic;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;
using Logger = ParadoxNotion.Services.Logger;
using ParadoxNotion;

namespace NodeCanvas.StateMachines
{

    ///<summary> Use FSMs to create state like behaviours</summary>
    [CreateAssetMenu(menuName = "ParadoxNotion/NodeCanvas/FSM Asset")]
    public class FSM : Graph
    {
        // Enum xác định cách gọi chuyển trạng thái
        public enum TransitionCallMode
        {
            Normal = 0, // Chuyển trạng thái bình thường
            Stacked = 1, // Đẩy trạng thái hiện tại vào stack trước khi chuyển
            Clean = 2, // Xóa stack trước khi chuyển
        }

        // Danh sách các node có thể cập nhật mỗi frame
        private List<IUpdatable> updatableNodes;
        // Các đối tượng nhận callback khi trạng thái thay đổi
        private IStateCallbackReceiver[] callbackReceivers;
        // Stack lưu trữ các trạng thái khi chuyển kiểu Stacked
        private Stack<FSMState> stateStack;
        // Cờ để xác định có vào trạng thái bắt đầu hay không
        private bool enterStartStateFlag;

        // Sự kiện khi vào, cập nhật, thoát, hoặc chuyển trạng thái
        public event System.Action<IState> onStateEnter;
        public event System.Action<IState> onStateUpdate;
        public event System.Action<IState> onStateExit;
        public event System.Action<IState> onStateTransition;

        ///<summary>Trạng thái hiện tại của FSM</summary>
        public FSMState currentState { get; private set; }
        ///<summary>Trạng thái trước đó của FSM</summary>
        public FSMState previousState { get; private set; }

        ///<summary>Tên trạng thái hiện tại. Null nếu không có</summary>
        public string currentStateName => currentState != null ? currentState.name : null;

        ///<summary>Tên trạng thái trước đó. Null nếu không có</summary>
        public string previousStateName => previousState != null ? previousState.name : null;

        // Các thuộc tính override từ Graph
        public override System.Type baseNodeType => typeof(FSMNode);
        public override bool requiresAgent => true;
        public override bool requiresPrimeNode => true;
        public override bool isTree => false;
        public override bool allowBlackboardOverrides => true;
        sealed public override bool canAcceptVariableDrops => false;
        public sealed override PlanarDirection flowDirection => PlanarDirection.Auto;

        ///----------------------------------------------------------------------------------------------

        // Khởi tạo graph, thu thập các node có thể cập nhật và callback receivers
        protected override void OnGraphInitialize()
        {
            // Có thể đang load bất đồng bộ
            ThreadSafeInitCall(GatherCallbackReceivers);
            updatableNodes = new List<IUpdatable>();
            for (var i = 0; i < allNodes.Count; i++)
            {
                if (allNodes[i] is IUpdatable)
                {
                    updatableNodes.Add((IUpdatable)allNodes[i]);
                }
            }
        }

        // Được gọi khi graph bắt đầu chạy
        protected override void OnGraphStarted()
        {
            stateStack = new Stack<FSMState>();
            enterStartStateFlag = true;
        }

        // Được gọi mỗi frame khi graph đang chạy
        protected override void OnGraphUpdate()
        {

            if (enterStartStateFlag)
            {
                // Dùng cờ để các node khác có thể xử lý khi graph bắt đầu
                enterStartStateFlag = false;
                EnterState((FSMState)primeNode, TransitionCallMode.Normal);
            }

            if (currentState != null)
            {

                // Cập nhật các node có thể cập nhật
                for (var i = 0; i < updatableNodes.Count; i++)
                {
                    updatableNodes[i].Update();
                }

                // Nếu trạng thái hiện tại bị null sau khi cập nhật, dừng FSM
                if (currentState == null) { Stop(false); return; }

                // Thực thi trạng thái hiện tại
                currentState.Execute(agent, blackboard);

                // Nếu trạng thái hiện tại bị null sau khi thực thi, dừng FSM
                if (currentState == null) { Stop(false); return; }

                // Gọi sự kiện cập nhật trạng thái nếu trạng thái đang chạy
                if (onStateUpdate != null && currentState.status == Status.Running)
                {
                    onStateUpdate(currentState);
                }

                // Nếu trạng thái hiện tại bị null sau khi gọi sự kiện, dừng FSM
                if (currentState == null) { Stop(false); return; }

                // Nếu trạng thái không còn chạy và không có kết nối ra
                if (currentState.status != Status.Running && currentState.outConnections.Count == 0)
                {
                    // Nếu có trạng thái trong stack thì pop và chuyển về nó
                    if (stateStack.Count > 0)
                    {
                        var popState = stateStack.Pop();
                        EnterState(popState, TransitionCallMode.Normal);
                        return;
                    }

                    // Nếu không còn node nào đang chạy thì dừng FSM
                    if (!updatableNodes.Any(n => n.status == Status.Running))
                    {
                        Stop(true);
                        return;
                    }
                }
            }

            // Nếu trạng thái hiện tại bị null, dừng FSM
            if (currentState == null)
            {
                Stop(false);
                return;
            }
        }

        // Được gọi khi FSM dừng lại
        protected override void OnGraphStoped()
        {
            if (currentState != null)
            {
                if (onStateExit != null)
                {
                    onStateExit(currentState);
                }
            }

            previousState = null;
            currentState = null;
            stateStack = null;
        }

        ///<summary>Chuyển sang trạng thái mới, truyền vào trạng thái và kiểu chuyển</summary>
        public bool EnterState(FSMState newState, TransitionCallMode callMode)
        {

            if (!isRunning)
            {
                Logger.LogWarning("Tried to EnterState on an FSM that was not running", LogTag.EXECUTION, this);
                return false;
            }

            if (newState == null)
            {
                Logger.LogWarning("Tried to Enter Null State", LogTag.EXECUTION, this);
                return false;
            }

            if (currentState != null)
            {
                if (onStateExit != null) { onStateExit(currentState); }
                currentState.Reset(false);
                if (callMode == TransitionCallMode.Stacked)
                {
                    stateStack.Push(currentState);
                    if (stateStack.Count > 5)
                    {
                        Logger.LogWarning("State stack exceeds 5. Ensure that you are not cycling stack calls", LogTag.EXECUTION, this);
                    }
                }
            }

            if (callMode == TransitionCallMode.Clean)
            {
                stateStack.Clear();
            }

            previousState = currentState;
            currentState = newState;

            if (onStateTransition != null) { onStateTransition(currentState); }
            if (onStateEnter != null) { onStateEnter(currentState); }
            currentState.Execute(agent, blackboard);
            return true;
        }

        ///<summary>Kích hoạt chuyển trạng thái theo tên. Trả về trạng thái đã tìm thấy và chuyển nếu có</summary>
        public FSMState TriggerState(string stateName, TransitionCallMode callMode)
        {

            var state = GetStateWithName(stateName);
            if (state != null)
            {
                EnterState(state, callMode);
                return state;
            }

            Logger.LogWarning("No State with name '" + stateName + "' found on FSM '" + name + "'", LogTag.EXECUTION, this);
            return null;
        }

        ///<summary>Lấy tất cả tên các trạng thái</summary>
        public string[] GetStateNames()
        {
            return allNodes.Where(n => n is FSMState).Select(n => n.name).ToArray();
        }

        ///<summary>Lấy trạng thái theo tên</summary>
        public FSMState GetStateWithName(string name)
        {
            return (FSMState)allNodes.Find(n => n is FSMState && n.name == name);
        }

        // Thu thập các callback receivers và đăng ký sự kiện trạng thái
        void GatherCallbackReceivers()
        {

            if (agent == null) { return; }

            callbackReceivers = agent.gameObject.GetComponents<IStateCallbackReceiver>();
            if (callbackReceivers.Length > 0)
            {
                onStateEnter += (x) => { foreach (var m in callbackReceivers) m.OnStateEnter(x); };
                onStateUpdate += (x) => { foreach (var m in callbackReceivers) m.OnStateUpdate(x); };
                onStateExit += (x) => { foreach (var m in callbackReceivers) m.OnStateExit(x); };
            }
        }

        // Lấy trạng thái trên cùng của stack (nếu có)
        public FSMState PeekStack()
        {
            return stateStack != null && stateStack.Count > 0 ? stateStack.Peek() : null;
        }


        ///----------------------------------------------------------------------------------------------
        ///---------------------------------------UNITY EDITOR-------------------------------------------
#if UNITY_EDITOR

        [UnityEditor.MenuItem("Tools/ParadoxNotion/NodeCanvas/Create/State Machine Asset", false, 1)]
        static void Editor_CreateGraph()
        {
            var newGraph = EditorUtils.CreateAsset<FSM>();
            UnityEditor.Selection.activeObject = newGraph;
        }
#endif

    }
}