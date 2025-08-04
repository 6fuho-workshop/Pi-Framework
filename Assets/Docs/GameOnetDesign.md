# Game Design Document: Onet

## ?? M?c ti�u

T?o m?t tr� ch?i **Onet (Pikachu n?i h�nh)** s? d?ng **Pi Framework (PF)**. Tr� ch?i n�y cho ph�p ng??i ch?i n?i c�c c?p h�nh gi?ng nhau theo lu?t n?i ???ng ?i kh�ng qu� 3 ?o?n th?ng.

---

## ?? C�c ch? ?? ch?i (Game Modes)

| T�n ch? ??     | M� t? |
|----------------|------|
| Classic Mode   | B?ng 10x16, gi?i h?n th?i gian, c�ng nhanh c�ng ?i?m cao |
| Relax Mode     | B?ng 8x10, kh�ng gi?i h?n th?i gian, d? ch?i |
| Adventure Mode | G?m nhi?u m�n ch?i li�n ti?p, c� v?t c?n, th�m power-up |

---

## ?? C?u tr�c h? th?ng

### PiActivity

M?i ch? ?? ch?i l� m?t `PiActivity` ri�ng:

- `GameOnetClassicActivity`
- `GameOnetRelaxActivity`
- `GameOnetAdventureActivity`

### Services

C�c service ch�nh (d�ng ServiceLocator ?? truy xu?t):

- `GameBoardService`: kh?i t?o v� qu?n l� d? li?u b?ng ch?i
- `GameMatchService`: ki?m tra kh? n?ng n?i 2 tile
- `ShuffleService`: khi kh�ng c�n ???ng ?i, x�o tr?n b?ng
- `TimerService`: ??m th?i gian trong Classic mode
- `ScoreService`: qu?n l� ?i?m s?
- `SfxService`: ph�t �m thanh khi n?i ?�ng, sai...

---

## ?? Lu?ng ch?i c? b?n

1. `PiActivity` kh?i t?o ? g?i `GameBoardService` ?? t?o board.
2. Ng??i ch?i ch?n 2 � ? g?i event `OnTileSelected`.
3. G?i `GameMatchService` ?? ki?m tra h?p l?.
4. N?u h?p l? ? x�a 2 tile, c?p nh?t ?i?m.
5. Ki?m tra c�n ???ng ?i kh�ng ? n?u kh�ng ? g?i `ShuffleService`.
6. K?t th�c khi b?ng tr?ng ? th?ng cu?c.

---

## ?? C�c class ch�nh

### GameTile

```csharp
public class GameTile {
    public TileType tileType;
    public Vector2Int position;
    public bool isVisible;
}