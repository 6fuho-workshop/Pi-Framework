using System;
using System.Collections.Generic;
using System.Linq;

namespace PF.DataStructures
{
    /// <summary>
    /// Represents a recursive tree structure where each node is defined by its own type.
    /// This interface is designed for logic-based trees, such as flow graphs, game state trees,
    /// or behavior trees, where nodes encapsulate specific logic and relationships.
    /// </summary>
    /// <typeparam name="T">The concrete node type implementing this interface.</typeparam>
    /// <remarks>
    /// Suitable for extending hierarchical logic structures where data is not the focus,
    /// but behavior and structure are. Enables tree traversal and structural introspection
    /// without assuming external data binding.
    /// </remarks>
    public interface ITreeNode<T> where T : class, ITreeNode<T>
    {
        /// <summary>Gets the parent node of this node.</summary>
        T Parent { get; }

        /// <summary>Gets the read-only list of child nodes.</summary>
        IReadOnlyList<T> Children { get; }

        /// <summary>Gets a value indicating whether this node is the root node.</summary>
        bool IsRoot { get; }

        /// <summary>Gets a value indicating whether this node is a leaf node (has no children).</summary>
        bool IsLeaf { get; }

        /// <summary>Adds a child node to this node.</summary>
        /// <param name="child">The child node to add.</param>
        /// <returns>This node (for method chaining).</returns>
        /// <exception cref="ArgumentNullException">Thrown when child is null.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the child already has a parent or when adding would create a cycle.</exception>
        T AddChild(T child);

        /// <summary>Removes a child node from this node.</summary>
        /// <param name="child">The child node to remove.</param>
        /// <returns>True if the child was removed; otherwise, false.</returns>
        bool RemoveChild(T child);
    }

    /// <inheritdoc cref="ITreeNode{TSelf}"/>
    public class TreeNode<T> : ITreeNode<T> where T : TreeNode<T>
    {
        #region Fields and Events

        private readonly List<T> _children = new();

        /// <summary>
        /// Event raised when a child node is added to this node.
        /// </summary>
        /// <remarks>The event argument contains the child node that was added.</remarks>
        public event EventHandler<T> ChildAdded;

        /// <summary>
        /// Event raised when a child node is removed from this node.
        /// </summary>
        /// <remarks>The event argument contains the child node that was removed.</remarks>
        public event EventHandler<T> ChildRemoved;

        #endregion

        #region Properties

        /// <inheritdoc/>
        public T Parent { get; private set; }

        /// <inheritdoc/>
        public IReadOnlyList<T> Children => _children;

        /// <inheritdoc/>
        public bool IsRoot => Parent == null;

        /// <inheritdoc/>
        public bool IsLeaf => _children.Count == 0;

        #endregion

        #region Constructors

        protected TreeNode() { }

        /// <inheritdoc/>
        public T AddChild(T child)
        {
            if (child == null) throw new ArgumentNullException(nameof(child));
            if (child.Parent != null)
                throw new InvalidOperationException("Child already has a parent.");

            // Prevent cycles - check if this node is already a descendant of the child
            if (child == (T)this)
                throw new InvalidOperationException("Cannot add a node as its own child.");

            // Check ancestors of this node to see if any match the child (more efficient)
            T ancestor = Parent;
            while (ancestor != null)
            {
                if (ancestor == child)
                    throw new InvalidOperationException("Cannot add an ancestor node as a child, as this would create a cycle in the tree.");
                ancestor = ancestor.Parent;
            }

            child.Parent = (T)this;
            _children.Add(child);
            OnChildAdded(child);
            return (T)this;
        }

        /// <inheritdoc/>
        public bool RemoveChild(T child)
        {
            if (child == null) return false;
            if (_children.Remove(child))
            {
                child.Parent = null;
                OnChildRemoved(child);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Removes all child nodes from this node.
        /// </summary>
        /// <returns>This node (for method chaining).</returns>
        public T ClearChildren()
        {
            var copied = new List<T>(_children);
            foreach (var child in copied)
            {
                RemoveChild(child);
            }
            return (T)this;
        }

        #endregion

        #region Protected Methods

        protected virtual void OnChildAdded(T child)
        {
            ChildAdded?.Invoke(this, child);
        }

        protected virtual void OnChildRemoved(T child)
        {
            ChildRemoved?.Invoke(this, child);
        }

        #endregion
    }

    /// <summary>
    /// Provides extension methods for ITreeNode implementations to enhance tree manipulation and traversal.
    /// </summary>
    public static class TreeNodeExtensions
    {
        /// <summary>
        /// Detaches a node from its parent, maintaining the node's subtree intact.
        /// </summary>
        /// <typeparam name="T">The node type.</typeparam>
        /// <param name="node">The node to detach.</param>
        /// <returns>The detached node.</returns>
        /// <exception cref="ArgumentNullException">Thrown when node is null.</exception>
        public static T Detach<T>(this ITreeNode<T> node) where T : class, ITreeNode<T>
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            node.Parent?.RemoveChild((T)node);
            return (T)node;
        }

        /// <summary>
        /// Traverses the tree recursively in a depth-first manner.
        /// </summary>
        /// <typeparam name="T">The node type.</typeparam>
        /// <param name="node">The node to start traversal from.</param>
        /// <returns>An enumerable sequence of nodes in depth-first order.</returns>
        /// <exception cref="ArgumentNullException">Thrown when node is null.</exception>
        public static IEnumerable<T> Traverse<T>(this ITreeNode<T> node) where T : class, ITreeNode<T>
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            yield return (T)node;
            foreach (var child in node.Children)
            {
                foreach (var descendant in child.Traverse())
                    yield return descendant;
            }
        }

        /// <summary>
        /// Traverses the tree using a breadth-first approach.
        /// </summary>
        /// <typeparam name="T">The node type.</typeparam>
        /// <param name="node">The node to start traversal from.</param>
        /// <returns>An enumerable sequence of nodes in breadth-first order.</returns>
        /// <exception cref="ArgumentNullException">Thrown when node is null.</exception>
        public static IEnumerable<T> TraverseBFS<T>(this ITreeNode<T> node) where T : class, ITreeNode<T>
        {
            var queue = new Queue<T>();
            queue.Enqueue((T)node);

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                yield return current;

                foreach (var child in current.Children)
                    queue.Enqueue(child);
            }
        }

        /// <summary>
        /// Finds the first node that matches the specified predicate.
        /// </summary>
        /// <typeparam name="T">The node type.</typeparam>
        /// <param name="node">The starting node for the search.</param>
        /// <param name="predicate">The predicate to match nodes against.</param>
        /// <returns>The first matching node, or null if no match is found.</returns>
        /// <exception cref="ArgumentNullException">Thrown when node or predicate is null.</exception>
        public static T Find<T>(this ITreeNode<T> node, Predicate<T> predicate) where T : class, ITreeNode<T>
        {
            return node.Traverse().FirstOrDefault(n => predicate(n));
        }

        /// <summary>
        /// Gets the path from the current node to the root of the tree.
        /// </summary>
        /// <typeparam name="T">The node type.</typeparam>
        /// <param name="node">The node to get the path for.</param>
        /// <returns>An enumerable of nodes representing the path to the root.</returns>
        /// <exception cref="ArgumentNullException">Thrown when node is null.</exception>
        public static IEnumerable<T> GetPathFromRoot<T>(this ITreeNode<T> node) where T : class, ITreeNode<T>
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            
            // Build the path
            var path = new Stack<T>();
            T current = (T)node;
            
            while (current != null)
            {
                path.Push(current);
                current = current.Parent;
            }
            
            // Return in root-to-node order
            return path;
        }

        /// <summary>
        /// Finds all sibling nodes of the current node.
        /// </summary>
        /// <typeparam name="T">The node type.</typeparam>
        /// <param name="node">The node to find siblings for.</param>
        /// <returns>An enumerable of sibling nodes.</returns>
        /// <exception cref="ArgumentNullException">Thrown when node is null.</exception>
        public static IEnumerable<T> FindSiblings<T>(this ITreeNode<T> node) where T : class, ITreeNode<T>
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            if (node.IsRoot)
                yield break;

            foreach (var child in node.Parent.Children)
            {
                if (!ReferenceEquals(child, node))
                    yield return child;
            }
        }

        /// <summary>
        /// Finds all ancestor nodes of the current node.
        /// </summary>
        /// <typeparam name="T">The node type.</typeparam>
        /// <param name="node">The node to find ancestors for.</param>
        /// <returns>An enumerable of ancestor nodes.</returns>
        /// <exception cref="ArgumentNullException">Thrown when node is null.</exception>
        public static IEnumerable<T> FindAncestors<T>(this ITreeNode<T> node) where T : class, ITreeNode<T>
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            
            T current = node.Parent;
            while (current != null)
            {
                yield return current;
                current = current.Parent;
            }
        }

        /// <summary>
        /// Gets the depth of the current node in the tree.
        /// </summary>
        /// <typeparam name="T">The node type.</typeparam>
        /// <param name="node">The node to get the depth for.</param>
        /// <returns>The depth of the node, with the root node at depth 0.</returns>
        /// <exception cref="ArgumentNullException">Thrown when node is null.</exception>
        public static int GetDepth<T>(this ITreeNode<T> node) where T : class, ITreeNode<T>
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            
            int depth = 0;
            var current = node;
            while (!current.IsRoot)
            {
                depth++;
                current = current.Parent;
            }
            return depth;
        }

        /// <summary>
        /// Gets the root node of the tree that the current node belongs to.
        /// </summary>
        /// <typeparam name="T">The node type.</typeparam>
        /// <param name="node">The node to get the root for.</param>
        /// <returns>The root node of the tree.</returns>
        /// <exception cref="ArgumentNullException">Thrown when node is null.</exception>
        public static T GetRoot<T>(this ITreeNode<T> node) where T : class, ITreeNode<T>
        {
            if (node == null) throw new ArgumentNullException(nameof(node));
            
            T current = (T)node;
            while (!current.IsRoot)
            {
                current = current.Parent;
            }
            return current;
        }
    }

    /// <summary>
    /// Represents an entire tree structure with a typed root node.
    /// Manages a collection of TreeNode objects and provides tree-level operations.
    /// </summary>
    /// <typeparam name="T">The concrete node type implementing ITreeNode.</typeparam>
    [Serializable]
    public class Tree<T> where T : class, ITreeNode<T>
    {
        #region Properties

        /// <summary>
        /// Gets or sets the root node of the tree.
        /// </summary>
        public T Root { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether this tree is empty (has no root).
        /// </summary>
        public bool IsEmpty => Root == null;

        /// <summary>
        /// Gets the total number of nodes in the tree.
        /// </summary>
        public int Count => IsEmpty ? 0 : Traverse().Count();

        #endregion

        #region Constructors

        /// <summary>
        /// Default constructor creating an empty tree.
        /// </summary>
        public Tree() { }

        /// <summary>
        /// Creates a new tree with the specified root node.
        /// </summary>
        /// <param name="root">The root node of the tree.</param>
        public Tree(T root)
        {
            SetRoot(root);
        }

        #endregion

        #region Tree Structure Methods

        /// <summary>
        /// Sets the root node of this tree, detaching it from any previous parent.
        /// </summary>
        /// <param name="root">The node to set as root.</param>
        /// <exception cref="ArgumentNullException">Thrown when root is null.</exception>
        public void SetRoot(T root)
        {
            Root = root ?? throw new ArgumentNullException(nameof(root));

            // Ensure the node is detached from any previous parent
            if (root.Parent != null)
                root.Detach();
        }

        /// <summary>
        /// Clears the tree by removing its root node.
        /// </summary>
        public void Clear()
        {
            Root = null;
        }

        #endregion

        #region Traversal Methods

        /// <summary>
        /// Traverses the tree in depth-first order.
        /// </summary>
        /// <returns>An enumerable sequence of all nodes in depth-first order.</returns>
        public IEnumerable<T> Traverse()
        {
            if (IsEmpty) yield break;
            foreach (var node in Root.Traverse())
                yield return node;
        }

        /// <summary>
        /// Traverses the tree in breadth-first order.
        /// </summary>
        /// <returns>An enumerable sequence of all nodes in breadth-first order.</returns>
        public IEnumerable<T> TraverseBFS()
        {
            if (IsEmpty) yield break;
            foreach (var node in Root.TraverseBFS())
                yield return node;
        }

        #endregion

        #region Node Finding Methods

        /// <summary>
        /// Finds the first node in the tree that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match nodes against.</param>
        /// <returns>The first matching node, or null if no match is found.</returns>
        public T FindFirst(Predicate<T> predicate)
        {
            if (IsEmpty) return null;
            return Root.Find(predicate);
        }

        /// <summary>
        /// Finds all nodes in the tree that match the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match nodes against.</param>
        /// <returns>An enumerable containing all matching nodes.</returns>
        public IEnumerable<T> FindAll(Predicate<T> predicate)
        {
            return Traverse().Where(n => predicate(n));
        }

        /// <summary>
        /// Gets all leaf nodes in the tree (nodes without children).
        /// </summary>
        /// <returns>An enumerable containing all leaf nodes.</returns>
        public IEnumerable<T> GetLeaves()
        {
            return Traverse().Where(n => n.IsLeaf);
        }

        #endregion

        #region Tree Analysis Methods

        /// <summary>
        /// Gets the maximum depth of the tree.
        /// </summary>
        /// <returns>The maximum depth of any node in the tree, or -1 if the tree is empty.</returns>
        public int GetMaxDepth()
        {
            if (IsEmpty) return -1;
            return CalculateHeight(Root);

            static int CalculateHeight(T node)
            {
                if (node.IsLeaf) return 0;

                int maxChildHeight = -1;
                foreach (var child in node.Children)
                    maxChildHeight = Math.Max(maxChildHeight, CalculateHeight(child));

                return maxChildHeight + 1;
            }
        }



        /// <summary>
        /// Determines whether the tree contains a node that matches the specified predicate.
        /// </summary>
        /// <param name="predicate">The predicate to match nodes against.</param>
        /// <returns>true if a matching node is found; otherwise, false.</returns>
        public bool Contains(Predicate<T> predicate)
        {
            return FindFirst(predicate) != null;
        }

        #endregion

        #region Object Overrides

        public override string ToString()
        {
            if (IsEmpty)
                return "Tree (Empty)";

            return $"Tree (Root: {Root}, Count: {Count})";
        }

        #endregion
    }
}