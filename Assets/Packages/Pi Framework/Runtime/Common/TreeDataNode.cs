using System;
using System.Collections.Generic;
using System.Linq;
namespace PiFramework.Common
{
    /// <summary>
    /// Represents a generic tree node that stores a data object and maintains parent-child relationships.
    /// This interface is suitable for data-driven trees such as menu trees, config hierarchies,
    /// dialogue trees, or content organization systems.
    /// </summary>
    /// <typeparam name="TData">The type of data stored in the node.</typeparam>
    /// <remarks>
    /// Designed for scenarios where the tree's primary role is to structure and organize data.
    /// Supports tree traversal, hierarchy checks, and optionally modification (if exposed).
    /// </remarks>
    public interface ITreeDataNode<TData> : ITreeNode<ITreeDataNode<TData>>
    {
        /// <summary>Gets the data stored in this node.</summary>
        TData Data { get; }
    }

    /// <summary>
    /// Represents a data tree node with mutable operations.  
    /// Supports both concrete-child manipulation and generic interface-based calls.
    /// </summary>
    /// <typeparam name="TData">The type of data stored in the node.</typeparam>
    public class TreeDataNode<TData> : ITreeDataNode<TData>
    {
        private readonly List<TreeDataNode<TData>> _children = new();

        /// <summary>Gets or sets the parent node of this node.</summary>
        public TreeDataNode<TData> Parent { get; private set; }
        
        /// <summary>Gets the data stored in this node.</summary>
        public TData Data { get; }

        /// <summary>
        /// Event raised when a child node is added to this node.
        /// </summary>
        /// <remarks>The event argument contains the child node that was added.</remarks>
        public event EventHandler<TreeDataNode<TData>> ChildAdded;
        
        /// <summary>
        /// Event raised when a child node is removed from this node.
        /// </summary>
        /// <remarks>The event argument contains the child node that was removed.</remarks>
        public event EventHandler<TreeDataNode<TData>> ChildRemoved;

        // IRead-only view exposed via interface
        IReadOnlyList<ITreeDataNode<TData>> ITreeNode<ITreeDataNode<TData>>.Children => _children.Cast<ITreeDataNode<TData>>().ToList().AsReadOnly();
        ITreeDataNode<TData> ITreeNode<ITreeDataNode<TData>>.Parent => Parent;

        public bool IsRoot => Parent == null;
        public bool IsLeaf => _children.Count == 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="TreeDataNode{TData}"/> class.
        /// </summary>
        /// <param name="data">The data to associate with this node.</param>
        /// <exception cref="ArgumentNullException">Thrown when data is null.</exception>
        public TreeDataNode(TData data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }

        /// <summary>
        /// Primary mutable add: adds a concrete child node.
        /// </summary>
        public TreeDataNode<TData> AddChild(TreeDataNode<TData> child)
        {
            if (child == null) throw new ArgumentNullException(nameof(child));
            if (child.Parent != null)
                throw new InvalidOperationException("Child already has a parent.");

            // Prevent cycles: ensure child is not ancestor of this
            var ancestor = this;
            while (ancestor != null)
            {
                if (ancestor == child)
                    throw new InvalidOperationException("Cannot add ancestor as child (would create cycle).");
                ancestor = ancestor.Parent;
            }

            _children.Add(child);
            child.Parent = this;
            ChildAdded?.Invoke(this, child);
            return this;
        }

        /// <summary>
        /// Interface-facing add: accepts the interface, attempts cast to concrete and delegates.
        /// </summary>
        public ITreeDataNode<TData> AddChild(ITreeDataNode<TData> child)
        {
            if (child == null) throw new ArgumentNullException(nameof(child));

            if (child is TreeDataNode<TData> concrete)
            {
                AddChild(concrete);
                return concrete;
            }

            throw new ArgumentException($"Child must be a {nameof(TreeDataNode<TData>)} instance.", nameof(child));
        }

        /// <summary>
        /// Primary mutable remove: removes a concrete child.
        /// </summary>
        public bool RemoveChild(TreeDataNode<TData> child)
        {
            if (child == null) return false;
            if (_children.Remove(child))
            {
                child.Parent = null;
                ChildRemoved?.Invoke(this, child);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Interface-facing remove: accepts interface and delegates if concrete.
        /// </summary>
        public bool RemoveChild(ITreeDataNode<TData> child)
        {
            if (child == null) throw new ArgumentNullException(nameof(child));

            if (child is TreeDataNode<TData> concrete)
            {
                return RemoveChild(concrete);
            }

            throw new ArgumentException($"Child must be a {nameof(TreeDataNode<TData>)} instance.", nameof(child));
        }

        /// <summary>
        /// Clears all children safely, using the concrete remove (which raises events).
        /// </summary>
        public void ClearChildren()
        {
            var copy = new List<TreeDataNode<TData>>(_children);
            foreach (var c in copy)
                RemoveChild(c);
        }

        // Other helpers like GetPathFromRoot, Find, etc., can remain as before or via extensions.
    }

    /// <summary>
    /// Represents a data tree node with mutable operations.  
    /// Supports both concrete-child manipulation and generic interface-based calls.
    /// </summary>
    /// <typeparam name="TData">The type of data stored in the node.</typeparam>
    public class TreeData<TData> : Tree<ITreeDataNode<TData>>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TreeData{TData}"/> class with the specified root node.
        /// </summary>
        /// <param name="root">The root node of the tree.</param>
        public TreeData(ITreeDataNode<TData> root) : base(root)
        {
        }

        /// <summary>
        /// Traverses all data nodes and returns only their data.
        /// </summary>
        /// <returns>An enumerable containing the data from all nodes.</returns>
        public IEnumerable<TData> TraverseData()
        {
            return Root.Traverse().Select(node => node.Data);
        }

        /// <summary>
        /// Finds a node containing data that satisfies the specified condition.
        /// </summary>
        /// <param name="predicate">The function to test data against.</param>
        /// <returns>The first node with matching data, or null if not found.</returns>
        public ITreeDataNode<TData> FindByData(Func<TData, bool> predicate)
        {
            return Root.Traverse().FirstOrDefault(node => predicate(node.Data));
        }

        /// <summary>
        /// Finds all nodes containing data that satisfies the specified condition.
        /// </summary>
        /// <param name="predicate">The function to test data against.</param>
        /// <returns>A list of all nodes with matching data.</returns>
        public List<ITreeDataNode<TData>> FindAllByData(Func<TData, bool> predicate)
        {
            return Root.Traverse().Where(node => predicate(node.Data)).ToList();
        }
    }

    ///<summary>
    /// Utility class to build TreeDataNode from nested data structure.
    /// </summary>
    public static class TreeDataBuilder
    {
        /// <summary>
        /// Builds a tree from root data and a function that returns children for each node.
        /// </summary>
        /// <typeparam name="TData">The type of data contained in the tree nodes.</typeparam>
        /// <param name="rootData">The data for the root node.</param>
        /// <param name="getChildren">A function that returns child data for a given node data.</param>
        /// <returns>The root node of the built tree.</returns>
        public static TreeDataNode<TData> Build<TData>(TData rootData, Func<TData, IEnumerable<TData>> getChildren)
        {
            var rootNode = new TreeDataNode<TData>(rootData);
            BuildRecursive(rootNode, getChildren);
            return rootNode;
        }

        /// <summary>
        /// Recursively builds the tree by adding children to the given node.
        /// </summary>
        /// <typeparam name="TData">The type of data contained in the tree nodes.</typeparam>
        /// <param name="parentNode">The parent node to add children to.</param>
        /// <param name="getChildren">A function that returns child data for a given node data.</param>
        private static void BuildRecursive<TData>(TreeDataNode<TData> parentNode, Func<TData, IEnumerable<TData>> getChildren)
        {
            foreach (var childData in getChildren(parentNode.Data))
            {
                var childNode = new TreeDataNode<TData>(childData);
                parentNode.AddChild(childNode);
                BuildRecursive(childNode, getChildren);
            }
        }
    }
}