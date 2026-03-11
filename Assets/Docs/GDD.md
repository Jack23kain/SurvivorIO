# Game Design Document — SurvivorIO

**Version:** 0.1
**Date:** 2026-03-11
**Genre:** Top-Down Auto-Battle Survival
**Platform:** Mobile (Portrait), PC
**Inspiration:** Vampire Survivors, Survivor.io

---

## 1. Game Overview

SurvivorIO is a top-down auto-battle survival game where a lone player character is dropped into an arena and must survive endless waves of enemies for as long as possible. The player controls only movement — attacks fire automatically. As the player levels up, they choose upgrades that shape their build and playstyle. The loop is short, addictive, and highly replayable.

---

## 2. Core Pillars

| Pillar | Description |
|--------|-------------|
| **Survive** | Dodge swarms of enemies using only movement |
| **Evolve** | Choose upgrades every level to build a unique loadout |
| **Escalate** | Enemy density and difficulty ramp up over time |
| **Collect** | Gather XP gems and currency dropped by enemies |

---

## 3. Gameplay Loop

```
Start Run
  │
  ▼
Move & Dodge Enemies  ◄──────────────────────┐
  │                                           │
  ▼                                           │
Auto-Attack fires → Enemies die → Drop XP    │
  │                                           │
  ▼                                           │
Collect XP → Fill Level Bar                  │
  │                                           │
  ▼                                           │
Level Up → Choose 1 of 3 Skill Cards         │
  │                                           │
  └─────────────────────────────────────────►┘
  │
  ▼ (Player dies or time limit reached)
Defeat Screen → Show Stats → Confirm → Meta Loop
```

---

## 4. Player Character

- **Perspective:** Top-down, portrait orientation
- **Movement:** Virtual joystick (mobile) / WASD or Arrow Keys (PC)
- **Attacks:** Fully automatic — no player input required to fire
- **Starting Weapon:** TBD (e.g., basic projectile in forward direction or radial burst)
- **Stats:**
  - Max HP
  - Move Speed
  - Attack Damage
  - Attack Speed
  - Area of Effect (AoE radius)
  - Pickup Radius

---

## 5. Enemies

- **Behavior:** Continuously spawn at the edges of the screen and move toward the player
- **Variety:** Different enemy types with varying speed, HP, size, and damage
- **Scaling:** Spawn rate, HP, and damage increase over time
- **Drops:** XP gems, coins, and power-up pickups on death

### Enemy Tiers (initial)

| Tier | Description | Traits |
|------|-------------|--------|
| Basic | Small, fast, low HP | Swarm in large numbers |
| Brute | Slow, high HP | Tanks damage, high contact damage |
| Ranged | Keeps distance, fires projectiles | Requires repositioning |
| Elite | Boss-lite mini-elites | Appear at timed intervals |

---

## 6. XP & Leveling

- Enemies drop **XP gems** on death
- Gems are automatically collected when within pickup radius (or pulled toward player)
- Filling the XP bar triggers a **level-up event**
- On level-up: game pauses and player chooses **1 of 3 skill cards**
- XP required per level scales upward

---

## 7. Skill / Upgrade System

On each level-up, the player is presented with **3 randomly selected upgrade cards**. Cards can be:

- **New Weapon:** Adds a new auto-attack to the player's arsenal
- **Weapon Upgrade:** Enhances an existing weapon (damage, speed, area, count)
- **Passive Buff:** Improves a player stat (HP, speed, pickup radius, etc.)

### Example Skill Cards (from reference)

| Card | Effect |
|------|--------|
| Summon Frogs & Bats | Periodic area summons that damage nearby enemies |
| Lightning Strike | Calls lightning bolts on enemies randomly |
| Falling Grenades | Grenades drop on the map dealing AoE damage |

### Upgrade Tiers
Each skill has multiple upgrade levels (e.g., I → II → III → MAX). Once maxed, the card is replaced by another option or evolves into a stronger form.

---

## 8. Pickups & Collectibles

| Pickup | Effect |
|--------|--------|
| XP Gem | Fills the level bar |
| Coin | Meta currency for upgrades between runs |
| Health Orb | Restores a portion of player HP |
| Magnet | Pulls all nearby XP gems instantly |
| Chest | Random item/upgrade drop |

---

## 9. Map & Environment

- **Layout:** Top-down tile-based arena (reference shows road/intersection theme)
- **Size:** Larger than the screen — player can move freely through the map
- **Camera:** Follows the player, portrait-oriented viewport
- **Obstacles:** Optional static obstacles (walls, barriers) that block movement/projectiles
- **Chapters:** Different map themes per chapter (e.g., City Streets → Industrial Zone → etc.)

---

## 10. Timer & Win/Lose Conditions

- **Timer:** Counts up from 0:00 — goal is to survive as long as possible
- **Lose Condition:** Player HP reaches 0 → Defeat Screen
- **Win Condition (Chapter Mode):** Survive a set duration (e.g., 10 minutes) → Chapter Clear
- **Personal Best:** Best survival time tracked per chapter

---

## 11. Defeat Screen

Displayed when the player dies or a chapter ends. Shows:

- Survived time
- Chapter number
- Personal best time
- Score (enemy kill count or weighted score)
- Collected coins/gems during the run
- **Confirm** button to return to meta loop

---

## 12. Meta Progression (Between Runs)

- Coins earned in runs are spent in a persistent upgrade menu
- Permanent stat upgrades carry across all runs (e.g., +Max HP, +Starting Damage)
- Possible unlocks: new starting weapons, new characters, new chapters

---

## 13. UI Layout (Portrait)

```
┌─────────────────────┐
│  [Pause] [Timer] [Score] │  ← HUD Bar
│  ████████░░░░░  XP Bar   │
├─────────────────────┤
│                     │
│     GAME WORLD      │
│   (player + enemies)│
│                     │
│                     │
├─────────────────────┤
│   [Joystick Area]   │  ← Input area (mobile)
└─────────────────────┘
```

**Level-Up Overlay:**
- Darkened background
- 3 skill cards displayed vertically or in a row
- Tap/click a card to select and resume

---

## 14. Audio (Placeholder Direction)

| Context | Style |
|---------|-------|
| Gameplay BGM | Upbeat, looping electronic/chiptune |
| Level Up | Short positive fanfare |
| Enemy Hit | Light impact SFX |
| Player Death | Dramatic sting |
| UI Buttons | Subtle click/pop |

---

## 15. Technical Notes (Unity)

- **Engine:** Unity (current project)
- **Orientation:** Portrait (1080×1920 reference)
- **Input:** Unity Input System (already configured — `InputSystem_Actions.inputactions` present)
- **Rendering:** 2D (Sprite Renderer / URP 2D)
- **Scene Structure:**
  - `GameScene` — main gameplay
  - `MenuScene` — main menu / meta
  - `UIScene` (additive) — persistent HUD

---

## 16. Milestones (Suggested)

| Milestone | Description |
|-----------|-------------|
| M1 | Player movement, camera follow, basic arena |
| M2 | Enemy spawning, pathfinding toward player, HP/death |
| M3 | Auto-attack system, first weapon |
| M4 | XP gems, level-up, skill card selection UI |
| M5 | 3–5 skills/weapons implemented |
| M6 | Timer, defeat screen, score tracking |
| M7 | Meta progression (coins, persistent upgrades) |
| M8 | Polish, audio, juice (screen shake, particles, VFX) |
| M9 | Chapter 1 complete, playtest build |

---

*This document will be updated as the project evolves.*
