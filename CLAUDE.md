# CLAUDE.md — Marrow

## What this project is

Marrow is a first-person, PS1-aesthetic survival-horror wave shooter built in Unity, inspired by COD Zombies. Core identity:

- Quake-style movement (bhop / bunny hopping) on a custom CharacterController mover — the player is FAST
- Throwable prop combat + a kill-charge wonder weapon economy (shotgun with self-boost recoil)
- Power station activation objective, train escape win condition
- Design pillar: a "Discord chill" multiplayer-feeling game — engaging without demanding constant attention
- Design pillar: **every weapon applies self-impulse to the player on fire** (pistol least → shotgun more → wonder weapon most). Impulse is a universal movement mechanic, not a shotgun gimmick.

The current design problem, confirmed by playtest: **the player outruns everything, so combat is optional and therefore boring.** The fix is spatial pressure (forward-biased spawning, intercept pathing, objective anchors), NOT enemy speed-scaling — speed-scaling loses to bhop compounding. Prioritize work that makes combat mandatory and fun over new content or maps.

**Returning to the project after a break? Read `marrow-leave-off.md` first** — it's the session-state
snapshot (what's half-finished, what's unverified, what editor wiring is outstanding). Then use the board
below for what to work on.

**Before picking up new work, check `marrow-build-board.md`.** It is the authoritative task list — ordered by what makes the game fun, not what's easiest — and its `▶ NEXT UP` section is the current answer to "what should I work on." Update it (not just this file) when a task is finished.

## Tech stack

- Unity **6000.0.32f1** (Unity 6), **URP**
- **ShaderGraph only** — Matthew does not hand-write HLSL. Effects go through ShaderGraph or particle systems.
- DOTween, NavMesh (NavMeshAgent enemies), CharacterController player
- Both legacy `Input.GetKey/GetAxis` and Input System `.inputactions` files exist in the project; gameplay scripts currently use **legacy Input**. Match existing usage unless asked to migrate.
- No test framework, no CI. Verification happens in the Unity editor — you cannot run the game. Flag anything that needs in-editor testing explicitly.

## Repo layout (the parts that matter)

```
Assets/
  Scenes/Gameplay.unity        ← main playable scene (also: Bar Hub, Highrise, Mockup, Testing)
  Scripts/
    Player Character/          ← PlayerMovement (Quake mover, private velocity), PlayerImpulse
                                  (external velocity layer w/ AddImpulse), AttackSystem (throwables,
                                  owns Mouse0), RaycastInteraction (E interact), PlayerStats, PlayerUI
    Enemy/                     ← EnemyStateMachine + EnemyStates/ (Idle, Chase, Attack, Stun, Dead),
                                  EnemyHealth, EnemyNav, EnemyAttack, EnemyAnimator, EnemyRagdoll, EnemyDrop
    Weapons/
      Shotgun/                 ← Shotgun.cs (pellet cone + boost impulse), ShotgunCharge.cs (kill→shell economy)
      ThunderGun/               ← legacy/stub, superseded by shotgun wonder weapon
      WeaponHandler.cs          ← OLD single-shotgun equip toggle (scroll wheel) — slated for replacement
      WeaponPickup.cs           ← OLD trigger-based shotgun pickup — slated for replacement
    ObjectPool.cs               ← single Queue, single prefab — multi-type refactor planned
    WaveManager.cs, Power.cs, Door.cs, PerkMachine.cs, WinCondition.cs, ThrowableObject.cs
```

## Current state (verify against repo, but as of Aug 2026)

- ✅ `RegisterKill()` IS wired into `EnemyHealth.TakeDamage` (this was the old #1 blocker — it's fixed)
- ✅ `DeadState` resets `timer = 0f` in `EnterState()` (stale-timer ragdoll bug fixed)
- ✅ `PlayerImpulse.AddImpulse()` exists as a separate velocity layer alongside `PlayerMovement`
- ✅ Throwable aim, velocity, and wall-tunneling fixed (Phase 0)
- ✅ Skeleton ragdoll-cap/vanish bug fixed (Phase 0)
- ✅ Forward-biased spawning is live in `WaveManager.cs` (`leadTime`, `aheadWeight`, `PickSpawnPoint()` weights points ahead of the player's travel direction). **But** `ChaseState.cs` still calls `EnemyNav.SetDestination(esm.PlayerTransform.position)` with no predicted lead — interception AI is NOT implemented despite spawn-ahead being done. Verify before assuming either is/isn't in place.
- ⏳ **Two-slot weapon system NOT yet integrated.** Five scripts were designed (`WeaponSO`, `WeaponInventory`, `WeaponFire`, `WeaponImpulse`, `WeaponPickup`) to replace `WeaponHandler` + old `WeaponPickup`, with diffs to `PlayerMovement`, `AttackSystem`, `RaycastInteraction`. They are not in the repo yet — check before assuming either system.
- ⏳ **Multi-enemy-type system NOT yet built.** Plan: `EnemyTypeSO` ScriptableObject + `EnemyIdentity`, `ObjectPool` refactored to `Dictionary<EnemyTypeSO, Queue>`, weighted spawn tables in `WaveManager`. Roster: Sprinter, Brute/tank, one-leg crawler, no-leg crawler, sprinter crawler.
- Progress is tracked on `marrow-build-board.md` (~75 tasks, 9 phases) — see the note above. Phase order: 0 blockers → 1 threat/combat fun → 2 economy → 3 power moment → 4 UI legibility → 5 sound → 6 juice → 7 map variety → 8 playtest/ship. **Phase 1 is the game** — combat threat work outranks everything else.

## Known bugs (open)

- Enemies clip through walls (NavMesh carving / missing obstacle colliders)
- Player passes through certain walls (collider gaps in the modular alleyway seams)
- Out-of-bounds colliders too short — boost mechanic clears them (raise at boundary; INSIDE the map, boost routes are a feature, keep them)
- Framerate hitch at run start (likely instantiate spike — pool enemies/props on load)
- UI communicates nothing: health, shells, wave, objective, power state all unreadable
- No sound implemented anywhere

## How to work in this codebase (Matthew's rules)

1. **Read the actual scripts before writing code.** Never write against assumed APIs. If a needed script isn't open, read it first. This is firm, not optional.
2. **Targeted diffs over rewrites.** When modifying an existing script, show specific line-level changes with a one-line reason each. Do not regenerate whole files that already exist.
3. **New functionality → new scripts.** Only touch existing scripts when integration genuinely requires it.
4. **Event-driven cross-system communication.** Systems talk via C# events (e.g., kill events → charge economy), not direct polling or singletons-grabbing-singletons.
5. **ScriptableObjects for data.** Per-type/per-weapon stats live in SOs (`WeaponSO`, planned `EnemyTypeSO`), not baked serialized fields.
6. **Ask before building when a design decision is open.** Matthew shares context progressively; one sharp clarifying question beats a wrong implementation.

## Gotchas learned the hard way

- `PlayerMovement.GroundMove` can zero out an impulse the same frame it's applied — any impulse into the main mover needs a **ground-lockout flag** for that frame. (Current shotgun avoids this by using the separate `PlayerImpulse` layer.)
- Pooled enemies keep stale state. **Reset fields in `EnterState()`, never rely on `ExitState()`** — pooled objects can skip exit paths.
- Inspector zeros are landmines: `killsPerCharge = 0` silently broke firing and produced NaN. Guard tunables with `[Min(1)]` + `Awake()` assertions.
- `EnemyAnimator` uses `CrossFade` with exact state-name strings (`idleAnim`, `chaseAnim`, `attackAnim`) — every enemy model's Animator Controller must use identical state names.
- Ragdoll rigs must be manually built per enemy prefab.
- Damage application in `AttackState` has been commented out before — verify it's live before tuning enemy damage.
- `ObjectPool.SpawnEnemy` historically called `GetEnemy` before its remaining-count guard (pool leak) — re-check when touching spawning.
- First-use shader variant compilation (URP/ShaderGraph) causes startup hitches — warm up shaders/pools if hitches appear.

## Scope guardrails

- Skyscraper map, jazz bar hub, and NPC dialogue systems are **deliberately backlogged.** Do not start them; a better map amplifies fun, it doesn't create it. Fix the core loop first.
- Wonder weapon mechanic is asset-agnostic — placeholder models/audio are fine; never block a mechanic on art.
- Free audio sources when needed: Freesound.org, Pixabay, Mixkit, Sonniss GDC bundles.

## Git

- Repo: `mbonnic-psx/Marrow` (renamed from `unity-game-project-marrow` — the old URL only survives via GitHub's redirect, so don't reintroduce it). Managed via GitHub Desktop.
- Phase work goes on its own branch off `main` (e.g. `phase-1-threat-combat`), not straight onto `main`
- Unity `.gitignore` is in place (Library/ etc. excluded); Plastic SCM was disconnected — don't reintroduce it
- Commit `.meta` files alongside their assets; never delete `.meta` files for assets that still exist
