using System;

namespace PF.Annotations
{
    /// <summary>
    /// Attribute to specify the desired Unity script execution order for a MonoBehaviour.
    /// Use with an editor tool to automatically set execution order based on this value.
    /// 
    /// <para>
    /// <b>Difference from Unity's DefaultExecutionOrder:</b><br/>
    /// - Unity's <c>DefaultExecutionOrder</c> only sets the order the first time. If a user changes the order manually
    /// in Project Settings &gt; Script Execution Order, that manual value will override the attribute and persist.<br/>
    /// - <c>ExecutionOrderAttribute</c> is designed for frameworks/tools that enforce execution order automatically.
    /// The <c>Order</c> value is always respected and reapplied by the framework/tool, so manual changes in the Editor
    /// will be overwritten. This ensures the execution order is always consistent with the codebase and cannot be
    /// accidentally changed by hand.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class ExecutionOrderAttribute : Attribute
    {
        /// <summary>
        /// The execution order value. Lower values execute earlier.
        /// This value is enforced by the framework and cannot be overridden by manual changes in the Unity Editor.
        /// </summary>
        public readonly int Order;

        /// <summary>
        /// Create a new ExecutionOrderAttribute.
        /// </summary>
        /// <param name="order">Execution order (lower = earlier).</param>
        public ExecutionOrderAttribute(int order)
        {
            Order = order;
        }
    }
}