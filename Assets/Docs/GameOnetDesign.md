# Game Design Document: Onet

## ?? M?c tiêu

T?o m?t trò ch?i **Onet (Pikachu n?i hình)** s? d?ng **Pi Framework (PF)**. Trò ch?i này cho phép ng??i ch?i n?i các c?p hình gi?ng nhau theo lu?t n?i ???ng ?i không quá 3 ?o?n th?ng.

---

## ?? Các ch? ?? ch?i (Game Modes)

| Tên ch? ??     | Mô t? |
|----------------|------|
| Classic Mode   | B?ng 10x16, gi?i h?n th?i gian, càng nhanh càng ?i?m cao |
| Relax Mode     | B?ng 8x10, không gi?i h?n th?i gian, d? ch?i |
| Adventure Mode | G?m nhi?u màn ch?i liên ti?p, có v?t c?n, thêm power-up |

---

## ?? C?u trúc h? th?ng

### PiActivity

M?i ch? ?? ch?i là m?t `PiActivity` riêng:

- `GameOnetClassicActivity`
- `GameOnetRelaxActivity`
- `GameOnetAdventureActivity`

### Services

Các service chính (dùng ServiceLocator ?? truy xu?t):

- `GameBoardService`: kh?i t?o và qu?n lý d? li?u b?ng ch?i
- `GameMatchService`: ki?m tra kh? n?ng n?i 2 tile
- `ShuffleService`: khi không còn ???ng ?i, xáo tr?n b?ng
- `TimerService`: ??m th?i gian trong Classic mode
- `ScoreService`: qu?n lý ?i?m s?
- `SfxService`: phát âm thanh khi n?i ?úng, sai...

---

## ?? Lu?ng ch?i c? b?n

1. `PiActivity` kh?i t?o ? g?i `GameBoardService` ?? t?o board.
2. Ng??i ch?i ch?n 2 ô ? g?i event `OnTileSelected`.
3. G?i `GameMatchService` ?? ki?m tra h?p l?.
4. N?u h?p l? ? xóa 2 tile, c?p nh?t ?i?m.
5. Ki?m tra còn ???ng ?i không ? n?u không ? g?i `ShuffleService`.
6. K?t thúc khi b?ng tr?ng ? th?ng cu?c.

---

## ?? Các class chính

### GameTile

```csharp
public class GameTile {
    public TileType tileType;
    public Vector2Int position;
    public bool isVisible;
}