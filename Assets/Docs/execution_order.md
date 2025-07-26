# Script Execution Order in Pi Framework

## Overview

Pi Framework uses a custom attribute, `ExecutionOrderAttribute`, to specify and enforce the execution order of MonoBehaviour scripts. This approach ensures that the script execution order is always consistent with the codebase and cannot be accidentally changed by hand.

---

## Why Not Use Unity's DefaultExecutionOrder?

Unity provides the `[DefaultExecutionOrder]` attribute to set the execution order of scripts. However, it has a key limitation:

- **DefaultExecutionOrder only sets the order the first time.**  
  If a user changes the order manually in **Project Settings > Script Execution Order**, that manual value will override the attribute and persist, even if you later change the attribute in code.

This can lead to inconsistencies, especially in team environments or when sharing code across projects.

---

## How Pi Framework Solves This

Pi Framework introduces `ExecutionOrderAttribute`:

- **Order is always enforced by the framework/tool.**
- **Manual changes in the Unity Editor will be overwritten.**
- This ensures the execution order is always consistent with the codebase and cannot be accidentally changed by hand.

**Example:**
```csharp
[ExecutionOrder(-100)] public class MyManager : MonoBehaviour { // ... }
```

A custom editor tool scans for this attribute and automatically sets the script execution order in Unity, overriding any manual changes.

---

## Summary

- Use `ExecutionOrderAttribute` for all critical scripts that require a specific execution order.
- Do not rely on Unity's Script Execution Order window for these scripts.
- The framework will always enforce the order defined in code, ensuring consistency and reliability.

---
