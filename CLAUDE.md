# SurvivorIO — Claude Code Instructions

## Project Overview
Vampire Survivors / Survivor.io-style top-down auto-battle mobile game built in Unity 2024 LTS with URP 17.3.0.

## Workflow Rules
- **Always push to GitHub automatically** after completing features — no need to ask.
- Use CoPlay MCP tools (`execute_script`, `get_unity_logs`, `check_compile_errors`, `save_scene`, etc.) to interact with the Unity Editor.
- After running `execute_script`, always verify with `get_unity_logs` to confirm success.

## Unity Setup
- **Engine**: Unity 2024 LTS
- **Render Pipeline**: URP 17.3.0
- **Input System**: New Input System (`InputActionAsset`)
- **UI**: TextMeshPro (always find and assign `TMP_FontAsset` or text will be invisible in-game)
- **Physics**: Rigidbody2D; use `OnTriggerEnter2D` / `OnTriggerStay2D` on enemies (larger collider) — not on daggers (too small/fast)

## Architecture

### Scene Structure
- `UICanvas` — all UI lives here
  - `TimerPanel` / `TimerText` — survival timer top center
  - `HPBarPanel` — bottom left player HP bar (UI)
  - `XPBarPanel` — bottom XP bar
  - `LevelUpPanel` — 3 card choice panel (pauses game)
  - `GameOverPanel` — game over overlay with Retry button

### Script Layout
```
Assets/Scripts/
  Camera/       CameraFollow.cs
  Combat/       AutoAttack.cs, Dagger.cs, DamageNumber.cs
  Enemy/        EnemyController.cs, EnemySpawner.cs
  Player/       PlayerController.cs, PlayerHealth.cs, PlayerHPBar.cs
  UI/           FloatingJoystick.cs, GameOverUI.cs, LevelUpUI.cs,
                SurvivalTimer.cs, XPBar.cs
  Utility/      BackgroundFollow.cs
  XP/           XPGem.cs, XPManager.cs
  Editor/       (one-time setup scripts — run via Tools menu or execute_script)
```

### Key GameObjects / Tags
- Player tagged `"Player"` — has `PlayerController`, `PlayerHealth`, `PlayerHPBar`, `AutoAttack`, `Rigidbody2D`, `CircleCollider2D` (radius 4)
- Enemy prefab — has `EnemyController`, `Rigidbody2D`, `BoxCollider2D` (isTrigger=true, ~6.66×9 world units)
- XP Gem prefab — has `XPGem`, `CircleCollider2D`

## Critical Patterns

### Hit Detection
Enemy's BoxCollider2D is a **trigger** and handles all collision detection in `EnemyController`:
- `OnTriggerEnter2D` — detects daggers (`Dagger` component), calls `TakeDamage` + `dagger.HitEnemy()`
- `OnTriggerStay2D` — detects player (`PlayerHealth`), deals 1 damage

### Button Wiring (Persistent vs Runtime)
- **Editor scripts**: use `UnityEditor.Events.UnityEventTools.AddPersistentListener` for buttons that must survive scene save (e.g., Retry button).
- **Runtime MonoBehaviours**: wire buttons in `Awake()` via `AddListener` — do NOT rely on editor-script listeners for gameplay buttons (they don't persist).

### Pausing
`Time.timeScale = 0f` on level-up and game over. Always reset to `1f` on resume/retry.

### XP Bar
Starts empty (`SetRatio(0f)` in `XPManager.Start()`). Uses `Image.Type.Filled` + `fillAmount`.

### HP Bar (World-Space)
Child of Player, uses `SpriteRenderer` scale + position offset for left-to-right fill:
```csharp
fill.localScale    = new Vector3(BarLocalWidth * ratio, fill.localScale.y, 1f);
fill.localPosition = new Vector3(BarLocalWidth * (ratio - 1f) * 0.5f, fill.localPosition.y, 0f);
```

## Game Values
| Stat | Value |
|------|-------|
| Player Max HP | 20 |
| Enemy Max HP | 9 |
| Enemy contact damage | 1/touch |
| Dagger damage | 10 |
| Attack range (min) | 8 units |
| Enemy speed range | 2–5 (scales over 120s) |
| Spawn interval range | 1.5s–0.3s (scales over 120s) |
| Wave size range | 1–4 (scales over 120s) |
| Enemy despawn distance | 25 units |
| XP required formula | `10 * currentLevel` |
| XP gem value | 1 XP |
| Gem pickup radius | 2.5 units |

## Level-Up Skills
5 skills pool, 3 shown randomly per level-up:
1. Rapid Fire — fire rate ×0.75
2. Power Strike — dagger damage +5
3. Swift Feet — move speed +1.5
4. Wide Range — attack range +2
5. Vital Boost — restore 5 HP

## Game Design Reference
See `Assets/References/GameDesignReference.png` for UI layout reference (Survivor.io style).
