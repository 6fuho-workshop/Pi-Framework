D??i ?�y l� m?t t�i li?u **??y ??, r� r�ng, c� c?u tr�c** v? kh�i ni?m **`FlowNode`** � ?? chi ti?t ?? c? con ng??i l?n c�c model AI nh? Copilot/assistant hi?u, t?o, m? r?ng, debug v� t�ch h?p v�o Pi Framework (PF).

---

# FlowNode � T�i li?u thi?t k?

## 1. T?ng quan (Overview)

`FlowNode` l� abstraction nh? cho m?t **?i?m (node) trong lu?ng ?i?u ph?i logic** c?a ?ng d?ng/game. N� kh�ng g?n ch?t v?i UI, scene, hay state machine c? ??nh; thay v�o ?� ??i di?n cho m?t �ch?t ng? c?nh� trong m?t **flow tree** ??ng, c� th? l?ng nhau, r? nh�nh, c� focus duy nh?t nh?ng v?n gi? to�n b? ???ng d?n active.

M?c ti�u:

* Cho ph�p x�y d?ng workflow (game flow, UI flow, initialization, transition, validation, v.v.) m?t c�ch **modular**, **declarative**, v� **extensible**.
* H? tr? **inject logic m?i** v�o gi?a flow hi?n c� m� kh�ng s?a code c?.
* Cho ph�p **l?ng**, **r? nh�nh**, **?i l�i** (back), **nh?y** (goTo), v� **focus** s�u nh?t.
* C� th? d�ng ?? ?i?u ph?i c�c h? th?ng nh? loading, reward, offer validation, transitions, v.v.

---

## 2. Kh�i ni?m c?t l�i

### 2.1. FlowNode

* M?t node trong c�y ho?c c?u tr�c logic bi?u di?n m?t �giai ?o?n�, �b??c�, ho?c �context� ?ang ho?t ??ng.
* C� th? m? r?ng (k? th?a) ?? ch?a logic c? th? (th�ng qua `TreeNode<TSelf>` ki?u logic) ho?c ch? d�ng ?? ch?a metadata n?u c?n (th�ng qua `TreeDataNode<TData>` -> sau chuy?n th�nh FlowNode runtime).

### 2.2. C�y Flow (Flow Tree)

* T? ch?c c�c `FlowNode` th�nh c?u tr�c ph�n c?p (cha-con), th? hi?n c�c flow c� th? l?ng nhau.
* `Focus` l� node s�u nh?t ?ang ???c �ch� ��/active v? m?t ?i?u ph?i ch�nh; to�n b? c�c node tr�n ???ng t? root ??n focus l� **active path**.

### 2.3. Focus vs Active

* **Focused FlowNode**: node duy nh?t c� �focus� v�o th?i ?i?m nh?t ??nh (n?i logic ch�nh ?ang di?n ra ho?c ch? t??ng t�c).
* **Active FlowNode**: b?t k? node n�o n?m tr�n ???ng d?n t? root t?i focus. Ch�ng v?n ???c xem l� �trong ng? c?nh� nh?ng kh�ng ph?i ?i?m ?i?u ph?i s�u nh?t.

### 2.4. Branching / Stack / Jump

* Flow kh�ng ph?i lu�n tuy?n t�nh: c� th? r? nh�nh (win ? reward ? special offer), ?� l�n t?m th?i (push pause), back (pop), ho?c nh?y (goTo main menu).

---

## 3. M?i quan h? v?i c�c th�nh ph?n kh�c

### 3.1. TreeDataNode ? FlowNode

* C?u h�nh flow c� th? khai b�o b?ng `TreeDataNode<FlowNodeConfig>` (data-driven).
* Sau khi load c?u h�nh, m?t factory chuy?n th�nh c�c `FlowNode` th?c thi (logic nodes) v� c?u tr�c runtime ???c x�y d?ng.

---

## 4. API thi?t k? (m?u C#)

### 4.1. FlowNode logic (k? th?a t? generic tree logic)

```csharp
public abstract class FlowNode : TreeNode<FlowNode>
{
    public string Id { get; }
    public bool IsFocused { get; internal set; }
    public bool IsActive => true; // ph? thu?c v�o manager, n?m trong active path

    public FlowNode(string id) { Id = id; }

    public virtual void OnEnter() { }
    public virtual void OnExit() { }
    public virtual void OnFocus() { }
    public virtual void OnUnfocus() { }

    // Helpers
    public bool IsInActivePath(FlowNode focus)
    {
        var current = focus;
        while (current != null)
        {
            if (current == this) return true;
            current = current.Parent;
        }
        return false;
    }
}
```

### 4.2. FlowNodeManager (?i?u ph?i)

C�c ch?c n?ng ch�nh:

* `GoTo(id)` ho?c `FocusInto(node)`
* `Push(subNode)`
* `Pop()`
* `Back()` (theo l?ch s?)
* `Replace(current, newNode)`
* Query: `CurrentFocus`, `IsInActivePath(node)`

### 4.3. DSL ??nh ngh?a flow (v� d? fluent)

```csharp
FlowBuilder
    .Root("MainMenu")
    .Then("Gameplay")
        .When("Win", f => f
            .Then("ScoreSummary")
            .Then("RewardCollecting")
            .Then("TransitionToMainMenu")
        )
        .When("Lose", f => f.Then("LoseScreen"))
    .Build();
```

---

## 5. C?u h�nh (Data-driven)

### 5.1. V� d? JSON config

```json
{
  "id": "MainMenu",
  "children": [
    {
      "id": "OfferCheck",
      "children": [
        { "id": "SpecialOfferPopup" }
      ]
    }
  ]
}
```

### 5.2. FlowNodeConfig ? Build runtime

```csharp
class FlowNodeConfig {
    public string Id;
    public List<FlowNodeConfig> Children = new();
}

// recursive factory
FlowNode Build(FlowNodeConfig config)
{
    var node = new ConcreteFlowNode(config.Id);
    foreach (var childCfg in config.Children)
    {
        var child = Build(childCfg);
        node.AddChild(child);
    }
    return node;
}
```

---

## 6. T�nh n?ng n�ng cao v� patterns

### 6.1. Lock / Guard

M?t node c� th? �kh�a� b?n th�n ho?c parent ?? ng?n vi?c chuy?n focus ?i khi kh�ng ho�n th�nh:

```csharp
public class FlowNode {
    private int _lockCount = 0;
    public void Lock() => _lockCount++;
    public void Release() => _lockCount = Math.Max(0, _lockCount - 1);
    public bool CanExit => _lockCount == 0;
}
```

Manager n�n ki?m tra `CanExit` tr??c khi cho r?i focus.

### 6.2. Sub-flow t?m th?i (Anonymous/Transient)

Push m?t `FlowNode` kh�ng ??nh danh c?ng ?? x? l� validation ho?c preload:

```csharp
var validation = new FlowNode("EntryValidation");
flowManager.Push(validation);
await validation.RunAsyncCheck();
flowManager.Pop(); // tr? focus v? parent
```

### 6.3. Branching & History

L?u l?ch s? c�c focus ?? `Back()` ho?t ??ng:

* M?i l?n focus change, push v�o history stack.
* `Back()` s? pop v� focus v�o node tr??c ?� n?u c�n.

---

## 7. Use cases c? th?

### 7.1. Flow ch?i game

```
MainMenu
  ??? OfferCheck (focused)
Gameplay
  ??? RewardSequence
  ??? PauseMenu (pushed)
```

* Khi th?ng:

  * FocusInto("ScoreSummary") ? OnEnter
  * Then ? FocusInto("RewardCollecting") ? dynamic child �AdOffer� c� th? ???c push
  * Transition back to MainMenu via `GoTo("MainMenu")`
  * Khi v�o MainMenu, push �EntryValidation� sub-flow tr??c khi unlock t??ng t�c.

### 7.2. Loading + Async update

* FocusInto("Loading")
* Lock loading until Firebase update finishes
* On completion, Release lock ? manager c� th? transition ti?p

---

## 8. T�ch h?p AI / Copilot prompt hints (semantic)

Khi AI mu?n hi?u ho?c s?a flow, c� th? d�ng c�c prompt d?ng:

* �Hi?n t?i focus ?ang ? node `RewardCollecting`. Ti?p theo theo flow l� g�?�
* �Th�m m?t sub-flow ki?m tra server tr??c khi v�o `MainMenu` m� kh�ng thay ??i c�c node c?.�
* �N?u user thua, go to `LoseScreen`; n?u th?ng, ?i qua `ScoreSummary` ? `RewardCollecting` ? `SpecialOffer` tr??c khi tr? v? `MainMenu`.�

FlowNode tree c� th? serialize th�nh c?u tr�c JSON/DSL ?? AI ??c/s?a:

```json
{
  "focus": "RewardCollecting",
  "path": ["Gameplay", "ScoreSummary", "RewardCollecting"]
}
```

---

## 9. Debugging / Visualization

* Visualize b?ng GameObject hierarchy (v� d? m?i FlowNode c� m?t GameObject debug d??i root `DontDestroyOnLoad`).
* Hi?n th?:

  * `Focused` node v?i tag ho?c m�u
  * Active path
  * Lock state
  * History stack
* Logger n�n output d?ng:

  ```
  [Flow] Focus changed: Gameplay -> ScoreSummary
  [Flow] Pushed subflow: EntryValidation (parent MainMenu)
  [Flow] Back() to MainMenu (from SpecialOffer)
  ```

---

## 10. Best practices

* D�ng `FlowNode` ?? ?i?u ph?i **logic c� th? t�ch r?i**, kh�ng nh?i UI/animation tr?c ti?p v�o node; delegate ra service.
* Gi?i h?n `FlowNode` cho �giai ?o?n c� � ngh?a� � kh�ng bi?n m?i popup nh? th�nh m?t node tr? khi th?c s? c?n staging ho?c guard.
* D�ng data-driven config ?? version/control flow; runtime ch? build t? config.
* S? d?ng events (`OnEnter`, `OnExit`, focus change) thay v� polling.
* Lu�n ki?m tra `CanExit` n?u node c� lock logic (async operations).

---

## 11. M? r?ng

* `FlowConditionNode` (n�t c� ?i?u ki?n chuy?n)
* `ParallelFlowNode` (ch?y nhi?u child song song v� ch? ??ng b?)
* `FallbackNode` (n�t d? ph�ng khi child th?t b?i)
* `TimedFlowNode` (timeout / delay trong flow)
* `FlowDecorator` (g�i m?t node v?i behavior th�m nh? logging, retry, instrumentation)

---

## 12. Ph?n m? r?ng d? ph�ng

### 12.1. Flow DSL builder

Cho ph�p ??nh ngh?a flow b?ng code d? ??c:

```csharp
var flow = FlowBuilder
    .Start("Boot")
    .Then("MainMenu", m => m
        .BeforeEnter("OfferCheck")
        .Then("Gameplay", g => g
            .On("Win", w => w.Then("ScoreSummary").Then("RewardCollecting"))
            .On("Lose", l => l.Then("LoseScreen"))
        )
    )
    .Build();
```

### 12.2. Serialization support

* C� th? serialize `FlowNodeConfig` (id + children + optional metadata) ra JSON/YAML.
* Runtime: config ? instantiate concrete `FlowNode` subclasses.

---

## 13. Pitfalls & Tricky cases

* **Modifying tree trong s? ki?n**: lu�n copy list khi l?p v� raise event (?� x? l� trong implementation).
* **Focus b? m?t do `GoTo` kh�ng ki?m tra lock**: manager ph?i respect `CanExit`.
* **Deep nesting kh�ng c� visualization**: ph?i c� tool ?? hi?n th? active path + focus.
* **Ambiguous transition**: n�n r� r�ng ai/?i?u g� g?i `GoTo` ho?c `Push`, kh�ng ?? logic implicit g�y side-effect.

---

## 14. Summary ?? AI hi?u

`FlowNode` l� �semantic node� trong m?t **flow tree c� th? l?ng nhau v� r? nh�nh**, v?i:

* **Focus** ?i?u ph?i hi?n t?i
* **Active path** l� ng? c?nh r?ng h?n
* **Push/Pop/GoTo/Back** qu?n l� navigation
* **Lock/Guard** cho async/validation tr??c khi chuy?n ti?p
* **Data-driven definition + runtime instantiation**
* **Events** ?? soft-couple logic (enter/exit/focus change)
* **Extensible decorators/patterns** ?? th�m retry, timeout, parallelism

---

B?n mu?n t�i b?c c�i n�y th�nh m?t markdown file/README c� th? ??a v�o repo lu�n kh�ng?