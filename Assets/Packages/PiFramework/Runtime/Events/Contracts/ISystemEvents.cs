using PF;
using PF.Events;
using PF.Primitives;
using UnityEngine;

namespace PF.Contracts
{
    public interface ISystemEvents
    {
        // Unity lifecycle events
        IEvent FirstAwakeCalled { get; }
        IEvent LastAwakeCalled { get; }
        IEvent FirstStartCalled { get; }
        IEvent LastStartCalled { get; }
        IEvent FirstUpdateCalled { get; }
        IEvent LastUpdateCalled { get; }
        IEvent FirstFixedUpdateCalled { get; }
        IEvent LastFixedUpdateCalled { get; }
        IEvent FirstLateUpdateCalled { get; }
        IEvent LastLateUpdateCalled { get; }

        // Other events
        IEvent<ProgressJoinOp> AppLoading { get; }
        IEvent<JoinOp> ShuttingDown { get; }
        IEvent Restarting { get; }
        IEvent InitializedAfterSceneLoad { get; }
        IEvent AppQuittingPhase1 { get; }
        IEvent AppQuittingPhase2 { get; }
        IEvent AppQuittingPhase3 { get; }
    }
}