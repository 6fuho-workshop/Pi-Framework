using PF.Events;
using PF.Primitives;

namespace PF.Contracts
{
    public class JoinOpEvent: PiEvent<JoinOp> { }
    public class ProgressJoinOpEvent<T> : PiEvent<ProgressJoinOp<T>> where T : IProgressJoinOpReport { }
    
}