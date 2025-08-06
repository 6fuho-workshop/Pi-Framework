D??i ?ây là m?t tài li?u **??y ??, rõ ràng, có c?u trúc** v? khái ni?m **`FlowNode`** — ?? chi ti?t ?? c? con ng??i l?n các model AI nh? Copilot/assistant hi?u, t?o, m? r?ng, debug và tích h?p vào Pi Framework (PF).

---

# FlowNode — Tài li?u thi?t k?

## 1. T?ng quan (Overview)

`FlowNode` là abstraction nh? cho m?t **?i?m (node) trong lu?ng ?i?u ph?i logic** c?a ?ng d?ng/game. Nó không g?n ch?t v?i UI, scene, hay state machine c? ??nh; thay vào ?ó ??i di?n cho m?t “ch?t ng? c?nh” trong m?t **flow tree** ??ng, có th? l?ng nhau, r? nhánh, có focus duy nh?t nh?ng v?n gi? toàn b? ???ng d?n active.

M?c tiêu:

* Cho phép xây d?ng workflow (game flow, UI flow, initialization, transition, validation, v.v.) m?t cách **modular**, **declarative**, và **extensible**.
* H? tr? **inject logic m?i** vào gi?a flow hi?n có mà không s?a code c?.
* Cho phép **l?ng**, **r? nhánh**, **?i lùi** (back), **nh?y** (goTo), và **focus** sâu nh?t.
* Có th? dùng ?? ?i?u ph?i các h? th?ng nh? loading, reward, offer validation, transitions, v.v.

---

## 2. Khái ni?m c?t lõi

### 2.1. FlowNode

* M?t node trong cây ho?c c?u trúc logic bi?u di?n m?t “giai ?o?n”, “b??c”, ho?c “context” ?ang ho?t ??ng.
* Có th? m? r?ng (k? th?a) ?? ch?a logic c? th? (thông qua `TreeNode<TSelf>` ki?u logic) ho?c ch? dùng ?? ch?a metadata n?u c?n (thông qua `TreeDataNode<TData>` -> sau chuy?n thành FlowNode runtime).

### 2.2. Cây Flow (Flow Tree)

* T? ch?c các `FlowNode` thành c?u trúc phân c?p (cha-con), th? hi?n các flow có th? l?ng nhau.
* `Focus` là node sâu nh?t ?ang ???c “chú ý”/active v? m?t ?i?u ph?i chính; toàn b? các node trên ???ng t? root ??n focus là **active path**.

### 2.3. Focus vs Active

* **Focused FlowNode**: node duy nh?t có “focus” vào th?i ?i?m nh?t ??nh (n?i logic chính ?ang di?n ra ho?c ch? t??ng tác).
* **Active FlowNode**: b?t k? node nào n?m trên ???ng d?n t? root t?i focus. Chúng v?n ???c xem là “trong ng? c?nh” nh?ng không ph?i ?i?m ?i?u ph?i sâu nh?t.

### 2.4. Branching / Stack / Jump

* Flow không ph?i luôn tuy?n tính: có th? r? nhánh (win ? reward ? special offer), ?è lên t?m th?i (push pause), back (pop), ho?c nh?y (goTo main menu).

---

## 3. M?i quan h? v?i các thành ph?n khác

### 3.1. TreeDataNode ? FlowNode

* C?u hình flow có th? khai báo b?ng `TreeDataNode<FlowNodeConfig>` (data-driven).
* Sau khi load c?u hình, m?t factory chuy?n thành các `FlowNode` th?c thi (logic nodes) và c?u trúc runtime ???c xây d?ng.

---

## 4. API thi?t k? (m?u C#)

### 4.1. FlowNode logic (k? th?a t? generic tree logic)

```csharp
public abstract class FlowNode : TreeNode<FlowNode>
{
    public string Id { get; }
    public bool IsFocused { get; internal set; }
    public bool IsActive => true; // ph? thu?c vào manager, n?m trong active path

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

Các ch?c n?ng chính:

* `GoTo(id)` ho?c `FocusInto(node)`
* `Push(subNode)`
* `Pop()`
* `Back()` (theo l?ch s?)
* `Replace(current, newNode)`
* Query: `CurrentFocus`, `IsInActivePath(node)`

### 4.3. DSL ??nh ngh?a flow (ví d? fluent)

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

## 5. C?u hình (Data-driven)

### 5.1. Ví d? JSON config

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

## 6. Tính n?ng nâng cao và patterns

### 6.1. Lock / Guard

M?t node có th? “khóa” b?n thân ho?c parent ?? ng?n vi?c chuy?n focus ?i khi không hoàn thành:

```csharp
public class FlowNode {
    private int _lockCount = 0;
    public void Lock() => _lockCount++;
    public void Release() => _lockCount = Math.Max(0, _lockCount - 1);
    public bool CanExit => _lockCount == 0;
}
```

Manager nên ki?m tra `CanExit` tr??c khi cho r?i focus.

### 6.2. Sub-flow t?m th?i (Anonymous/Transient)

Push m?t `FlowNode` không ??nh danh c?ng ?? x? lý validation ho?c preload:

```csharp
var validation = new FlowNode("EntryValidation");
flowManager.Push(validation);
await validation.RunAsyncCheck();
flowManager.Pop(); // tr? focus v? parent
```

### 6.3. Branching & History

L?u l?ch s? các focus ?? `Back()` ho?t ??ng:

* M?i l?n focus change, push vào history stack.
* `Back()` s? pop và focus vào node tr??c ?ó n?u còn.

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
  * Then ? FocusInto("RewardCollecting") ? dynamic child “AdOffer” có th? ???c push
  * Transition back to MainMenu via `GoTo("MainMenu")`
  * Khi vào MainMenu, push “EntryValidation” sub-flow tr??c khi unlock t??ng tác.

### 7.2. Loading + Async update

* FocusInto("Loading")
* Lock loading until Firebase update finishes
* On completion, Release lock ? manager có th? transition ti?p

---

## 8. Tích h?p AI / Copilot prompt hints (semantic)

Khi AI mu?n hi?u ho?c s?a flow, có th? dùng các prompt d?ng:

* “Hi?n t?i focus ?ang ? node `RewardCollecting`. Ti?p theo theo flow là gì?”
* “Thêm m?t sub-flow ki?m tra server tr??c khi vào `MainMenu` mà không thay ??i các node c?.”
* “N?u user thua, go to `LoseScreen`; n?u th?ng, ?i qua `ScoreSummary` ? `RewardCollecting` ? `SpecialOffer` tr??c khi tr? v? `MainMenu`.”

FlowNode tree có th? serialize thành c?u trúc JSON/DSL ?? AI ??c/s?a:

```json
{
  "focus": "RewardCollecting",
  "path": ["Gameplay", "ScoreSummary", "RewardCollecting"]
}
```

---

## 9. Debugging / Visualization

* Visualize b?ng GameObject hierarchy (ví d? m?i FlowNode có m?t GameObject debug d??i root `DontDestroyOnLoad`).
* Hi?n th?:

  * `Focused` node v?i tag ho?c màu
  * Active path
  * Lock state
  * History stack
* Logger nên output d?ng:

  ```
  [Flow] Focus changed: Gameplay -> ScoreSummary
  [Flow] Pushed subflow: EntryValidation (parent MainMenu)
  [Flow] Back() to MainMenu (from SpecialOffer)
  ```

---

## 10. Best practices

* Dùng `FlowNode` ?? ?i?u ph?i **logic có th? tách r?i**, không nh?i UI/animation tr?c ti?p vào node; delegate ra service.
* Gi?i h?n `FlowNode` cho “giai ?o?n có ý ngh?a” — không bi?n m?i popup nh? thành m?t node tr? khi th?c s? c?n staging ho?c guard.
* Dùng data-driven config ?? version/control flow; runtime ch? build t? config.
* S? d?ng events (`OnEnter`, `OnExit`, focus change) thay vì polling.
* Luôn ki?m tra `CanExit` n?u node có lock logic (async operations).

---

## 11. M? r?ng

* `FlowConditionNode` (nút có ?i?u ki?n chuy?n)
* `ParallelFlowNode` (ch?y nhi?u child song song và ch? ??ng b?)
* `FallbackNode` (nút d? phòng khi child th?t b?i)
* `TimedFlowNode` (timeout / delay trong flow)
* `FlowDecorator` (gói m?t node v?i behavior thêm nh? logging, retry, instrumentation)

---

## 12. Ph?n m? r?ng d? phòng

### 12.1. Flow DSL builder

Cho phép ??nh ngh?a flow b?ng code d? ??c:

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

* Có th? serialize `FlowNodeConfig` (id + children + optional metadata) ra JSON/YAML.
* Runtime: config ? instantiate concrete `FlowNode` subclasses.

---

## 13. Pitfalls & Tricky cases

* **Modifying tree trong s? ki?n**: luôn copy list khi l?p và raise event (?ã x? lý trong implementation).
* **Focus b? m?t do `GoTo` không ki?m tra lock**: manager ph?i respect `CanExit`.
* **Deep nesting không có visualization**: ph?i có tool ?? hi?n th? active path + focus.
* **Ambiguous transition**: nên rõ ràng ai/?i?u gì g?i `GoTo` ho?c `Push`, không ?? logic implicit gây side-effect.

---

## 14. Summary ?? AI hi?u

`FlowNode` là “semantic node” trong m?t **flow tree có th? l?ng nhau và r? nhánh**, v?i:

* **Focus** ?i?u ph?i hi?n t?i
* **Active path** là ng? c?nh r?ng h?n
* **Push/Pop/GoTo/Back** qu?n lý navigation
* **Lock/Guard** cho async/validation tr??c khi chuy?n ti?p
* **Data-driven definition + runtime instantiation**
* **Events** ?? soft-couple logic (enter/exit/focus change)
* **Extensible decorators/patterns** ?? thêm retry, timeout, parallelism

---

B?n mu?n tôi b?c cái này thành m?t markdown file/README có th? ??a vào repo luôn không?