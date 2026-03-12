# SurvivorIO — Skill System Reference

## How It Works

On every level-up, `XPManager.AddXP()` calls `LevelUpUI.Show()`, which:
1. Pauses the game (`Time.timeScale = 0f`)
2. Picks 3 unique random skills from the pool
3. Displays them as card buttons
4. On card click → `SelectCard(index)` → invokes the skill's `apply` lambda → resumes (`Time.timeScale = 1f`)

Skill logic lives entirely in `LevelUpUI.Awake()` — `apply` lambdas lazily find the Player at the moment they fire, so Awake order doesn't matter.

---

## Current Skill Pool (`LevelUpUI.cs`)

| # | Name | Description | Method Called |
|---|------|-------------|---------------|
| 0 | Rapid Fire | Fire rate ×0.75 (min 0.2s) | `AutoAttack.UpgradeFireRate()` |
| 1 | Power Strike | Dagger damage +5 | `AutoAttack.UpgradeDamage()` |
| 2 | Swift Feet | Move speed +1.5 | `PlayerController.UpgradeSpeed()` |
| 3 | Wide Range | Attack range +2 | `AutoAttack.UpgradeRange()` |
| 4 | Vital Boost | Restore 5 HP | `PlayerHealth.Heal(5)` |

---

## Upgradeable Stats (Current Defaults)

| Stat | Default | Upgrade Method | Per-Upgrade Delta |
|------|---------|---------------|-------------------|
| Fire rate | 0.8s | `UpgradeFireRate()` | ×0.75 (min 0.2s) |
| Dagger damage | 10 | `UpgradeDamage()` | +5 |
| Attack range | 8 units | `UpgradeRange()` | +2 |
| Move speed | 5 | `UpgradeSpeed()` | +1.5 |
| Player HP | 20 max | `Heal(int)` | +5 restore |

---

## How to Add a New Skill

### Step 1 — Add an upgrade method to the target component

Example in `AutoAttack.cs`:
```csharp
public void UpgradeMultishot() { projectilesPerShot++; }
```

### Step 2 — Add a `Skill` entry to the array in `LevelUpUI.Awake()`

```csharp
new Skill {
    title = "Multishot",
    desc  = "Fire an extra dagger per attack",
    apply = () => Player()?.GetComponent<AutoAttack>()?.UpgradeMultishot()
},
```

### Step 3 — Update the index list in `LevelUpUI.Show()`

The list `{ 0, 1, 2, 3, 4 }` must match the length of the `skills` array. If you add a 6th skill, change it to `{ 0, 1, 2, 3, 4, 5 }`.

> The card UI always shows exactly 3 choices. You can have any number of skills in the pool — the more skills, the less likely duplicates in a run.

---

## XP & Leveling Formula

```
XP required for level N = 10 × N
```

| Level | XP to next level |
|-------|-----------------|
| 1 | 10 |
| 2 | 20 |
| 3 | 30 |
| N | N × 10 |

XP is earned by collecting gems dropped by enemies. Each gem = 1 XP. Gems auto-pull to the player within **2.5 units** and are collected at **0.15 units**.

---

## Files Involved

| File | Role |
|------|------|
| `Scripts/UI/LevelUpUI.cs` | Skill pool definition, card UI, pause/unpause |
| `Scripts/XP/XPManager.cs` | Tracks XP + level, triggers `LevelUpUI.Show()` |
| `Scripts/XP/XPGem.cs` | Gem pickup → calls `XPManager.AddXP()` |
| `Scripts/Combat/AutoAttack.cs` | Upgrade methods: fire rate, damage, range |
| `Scripts/Player/PlayerController.cs` | Upgrade method: move speed |
| `Scripts/Player/PlayerHealth.cs` | Heal method used by Vital Boost |
