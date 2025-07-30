using System;
using System.Collections.Generic;
using UnityEngine;
using PiFramework.Timing;

namespace PiFramework
{
    public enum OperationState
    {
        Inactive,
        Delayed,
        Running,
        Paused,
        Stopped,
    }

    [System.Serializable]
    public class PiOperation
    {
        [SerializeField] protected List<PiOperation> children = new();
        [SerializeField] internal float delay;
        protected PiOperation parent;

        // Events
        public event Action onStarted;
        public event Action onStopped;
        //public event Action<string> onFailed;
        public event Action<float> onProgressUpdated;

        [SerializeField] protected float _progress = 0f;
        [SerializeField] protected string operationName = "";

        // Properties
        public float progress => _progress;
        public OperationState state { get; internal set; } = OperationState.Inactive;
        public bool isRunning => state == OperationState.Running;
        public bool isStopped => state == OperationState.Stopped;
        public bool isActive => state != OperationState.Inactive;
        public bool hasChildren => children.Count > 0;
        public string name => operationName;
        public int childCount => children.Count;

        

        // Constructor
        internal PiOperation(string name = "")
        {
            operationName = string.IsNullOrEmpty(name) ? GetType().Name : name;
        }

        // Public methods
        public virtual void Start()
        {
            if (isActive) return;

            state = OperationState.Running;
            onStarted?.Invoke();

            // Execute the operation logic
            Execute();
        }

        public PiOperation SetDelay(float delay)
        {
            this.delay = delay;
            return this;
        }

        // Virtual method để override logic thực thi
        protected virtual void Execute()
        {
            // Default implementation - just complete immediately
            // Override this method to implement custom logic
            Complete();
        }

        public void AddChild(PiOperation child)
        {
            if (child == null) return;

            child.parent = this;
            children.Add(child);

            child.onStopped += () => OnChildCompleted(child);
            //child.onFailed += (error) => OnChildFailed(child, error);

            // Auto-start child if parent is running
            if (state == OperationState.Running && child.isActive)
            {
                child.Start();
            }
        }

        public void RemoveChild(PiOperation child)
        {
            if (children.Remove(child))
            {
                child.parent = null;
            }
        }

        public void Update(float deltaTime = 0f)
        {
            if (state != OperationState.Running) return;

            // Update children first
            for (int i = children.Count - 1; i >= 0; i--)
            {
                if (i < children.Count) // Safety check
                {
                    children[i].Update(deltaTime);
                }
            }

            // Update self
            OnUpdate(deltaTime);

            // Check if can complete
            CheckCompletion();
        }

        // Virtual method để override update logic
        protected virtual void OnUpdate(float deltaTime)
        {
            // Override this for custom update logic
        }

        // Method để manually hoàn thành operation
        public void Complete()
        {
            if (state != OperationState.Running) return;

            state = OperationState.Stopped;
            _progress = 1f;

            onStopped?.Invoke();

            // Notify parent if exists
            parent?.OnChildCompleted(this);
        }

        /*
         * Fail là việc nội bộ của operation không cần thông báo cho parent
        public void Fail(string error)
        {
            if (state != OperationState.Running) return;

            state = OperationState.Failed;
            onFailed?.Invoke(error);

            // Cancel all children
            foreach (var child in children.ToArray())
            {
                child.Cancel();
            }
            children.Clear();

            // Notify parent
            parent?.OnChildFailed(this, error);
        }
        */

        public void Cancel()
        {
            if (state == OperationState.Stopped) return;

            state = OperationState.Stopped;

            // Cancel all children
            foreach (var child in children.ToArray())
            {
                child.Cancel();
            }
            children.Clear();
        }

        public void Pause()
        {
            if (state != OperationState.Running) return;

            state = OperationState.Paused;

            /*
             * Không nên tự động pause và resume children vì có thể xung đột với tác vụ pause/resume của chúng
            foreach (var child in children)
            {
                child.Pause();
            }
            */
        }

        public void Resume()
        {
            if (state != OperationState.Paused) return;

            state = OperationState.Running;

            /*
             * Không nên tự động pause và resume children
            foreach (var child in children)
            {
                child.Resume();
            }
            */
        }

        // Protected methods
        protected virtual void OnChildCompleted(PiOperation child)
        {
            RemoveChild(child);
            CheckCompletion();
        }

        protected virtual void OnChildFailed(PiOperation child, string error)
        {
            RemoveChild(child);
            // Default: log warning but don't fail parent
            Debug.LogWarning($"Child operation '{child.name}' failed: {error}");
        }

        protected virtual void CheckCompletion()
        {
            // Clean up completed/failed children
            children.RemoveAll(child => child.isStopped);

            // Can complete if no active children and operation allows it
            if (children.Count == 0 && CanComplete())
            {
                Complete();
            }
        }

        protected virtual bool CanComplete()
        {
            // Override this to add custom completion conditions
            return true;
        }

        protected void UpdateProgress(float value)
        {
            _progress = Mathf.Clamp01(value);
            onProgressUpdated?.Invoke(_progress);
        }

        // Utility methods
        public void WaitForChildren()
        {
            // This method can be called to explicitly wait
            // Implementation depends on execution context
        }

        public bool HasActiveChildren()
        {
            return children.Exists(child => child.isRunning || child.isActive);
        }

        public List<PiOperation> GetActiveChildren()
        {
            return children.FindAll(child => child.isRunning || child.isActive);
        }

        public List<PiOperation> GetAllChildren()
        {
            return new List<PiOperation>(children);
        }

        // Debug utilities
        public override string ToString()
        {
            return $"{operationName} ({state}) - Progress: {_progress:P0} - Children: {children.Count}";
        }

        public void PrintHierarchy(int indent = 0)
        {
            string indentStr = new string(' ', indent * 2);
            Debug.Log($"{indentStr}{ToString()}");

            foreach (var child in children)
            {
                child.PrintHierarchy(indent + 1);
            }
        }
    }
}