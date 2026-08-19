# Marrow — Leave-Off Notes

**Last session: 19 Aug 2026.** Snapshot of where things stand so you can pick this back up cold.

> This is a *session state* doc, not a task list. `marrow-build-board.md` is still the source of truth
> for what to work on next. This file just explains where you stopped and what's half-finished.

---

## ⚠️ Read this first — the game does not spawn enemies right now

The enemy system was refactored to be data-driven, and **the editor wiring was never finished.**
`ObjectPool`'s enemy list is sized to 5 with all slots empty. Until you fill it in, no enemies spawn
at all. You'll get a red console error saying so rather than silent failure.

**Fix it with the checklist in "Unfinished setup" below.** Start there.

---

## Where the work lives

- **Branch: `phase-1-threat-combat`** — all of it. `main` is untouched, sitting at `d048091`.
- Pushed and in sync with `origin`.
- Repo was renamed to `mbonnic-psx/Marrow` (the old `unity-game-project-marrow` URL only survives
  via GitHub's redirect). Local remote already updated.

Commits, oldest first:

| Commit | What |
|---|---|
| `16bc8d0` | Enemy interception AI, enemy damage made live, damage vignette |
| `2bf783e` | Repo rename cleanup in CLAUDE.md |
| `a87a5f2` | Enemy attack telegraph (visible wind-up) |
| `dc1152b` | `EnemyTypeSO` multi-enemy-type system + pool leak fix |
| `f8287b8` | The six enemy type assets |
| `317cfb8` | `.meta` files for those assets |
| `572ec69` | Scene, committed mid-setup |

---

## What got done

### Enemies now intercept instead of trailing you
`EnemyStateMachine.PredictedPlayerPosition()` leads off a *smoothed* player velocity, clamps the lead
before sampling the NavMesh, and strips out velocity that's closing on the enemy — leading into a
player running at you just sends the enemy sprinting past. `ChaseState` paths to that point instead of
your live transform. `EnemyNav` also turns off `autoBraking`, which was making agents decelerate on
"arrival" and hover just short of you.

### Enemies actually deal damage
`AttackState`'s attack call had been commented out, so enemies were harmless. It's live now, with a
Chase(2.5) / Attack(3.2) hysteresis band — with one shared threshold the states flip-flopped every
tick against a moving player and the cooldown restarted before a swing could ever land.

**The double-hit bug is fixed.** Test Enemy's `attackAnim` carried an Animation Event calling
`Attack`, which double-dipped with the code call. Code is now the single authority (uniform across
enemy types instead of depending on per-clip event authoring), the event was removed, and
`EnemyAttack` guards on time-since-last-landed-hit so it can't recur.

### Damage vignette
`DamageVignette.cs` — red tunnel-vision closing in as health drops. Builds its own canvas, image and
gradient texture at runtime, so it needs no art and no scene setup. Full effect at 1/3 health, which
with 33 damage vs 99 health is the second hit of three. **Press `H` to fake a hit** for testing.
This is a debug stopgap; Phase 4 will want the real art pass.

### Attack telegraph
`EnemyTelegraph.cs` — the enemy tints red and swells across the 0.6s wind-up before each swing. It's
driven off the *same timer that applies the damage*, so retuning `attackWindup` moves the warning
with it and they can't drift apart. **Only the visible half is done** — the audio hook (`windupClip`)
is wired but there's no audio asset in the project yet.

### Enemy types are data, not code
`EnemyTypeSO` + `EnemyIdentity`; `ObjectPool` is now one queue per type; `WaveManager` picks types by
weighted roulette. Adding an enemy type is now an asset change.

Also fixed the **pool leak** flagged in CLAUDE.md — `SpawnEnemy` dequeued *before* its guards, so
every skipped spawn permanently dropped an enemy from the pool.

---

## The roster

Six assets in `Assets/ScriptableObjects/EnemyTypes/`. Each keeps the agent speed its prefab was
already tuned at; what the assets added is differentiation, since health/damage were an identical
99/33 across the whole roster before.

| Type | HP | Dmg | Speed | Lead | Weight | First wave |
|---|---|---|---|---|---|---|
| Skeleton | 99 | 33 | 8 | 0.5 | 1.0 | 1 |
| **Sprinter** | 45 | 20 | 10 | **0.9** | 0.6 | 2 |
| Brute | 400 | 55 | 4 | 0.15 | 0.25 | 4 |
| Sprinter Crawler | 35 | 18 | 6 | 0.9 | 0.35 | 5 |
| One Leg | 60 | 25 | 3.2 | 0.3 | **0** | 3 |
| No Legs | 70 | 25 | 2.4 | 0.2 | **0** | 3 |

- **Sprinter** has the highest lead in the roster — it's the type that genuinely cuts you off. At 45hp
  one shotgun blast should drop it. Enters wave 2 so wave 1 stays a clean baseline.
- **Brute** barely leads on purpose. It's a wall that denies a route, not an interceptor.
- **One Leg / No Legs are at weight 0 on purpose** — see below.

---

## Unfinished setup (do this when you come back)

### 1. Verify the type assets imported
Open `Assets/ScriptableObjects/EnemyTypes/`, click each of the 6 assets, check the **Prefab** field
shows a skeleton and not "None". Those asset files were hand-written YAML, so this is the one thing
that could have silently gone wrong.

### 2. Fill in the ObjectPool ← *this is why nothing spawns*
Select the **ObjectPool** object. The old "Enemy Prefab" slot is gone; there's an **Enemy Types**
list. Set size to **6** (it's currently 5) and drag in all six assets. Confirm **Pool Parent** is
still assigned.

### 3. Per working enemy prefab (Skeleton, Sprinter, Brute, Sprinter Crawler)
- **Player Layer** on `EnemyAttack` must be set, or that enemy can never hurt you (red console error names the prefab)
- **Add `EnemyTelegraph`** — only one prefab has it so far
- **Animator states must be named exactly** `idleAnim`, `chaseAnim`, `attackAnim`
- **Ragdoll rig** has to be built by hand per prefab

You do *not* need to add `EnemyIdentity` — the pool adds it at runtime.

### 4. The two crawler prefabs are just models
`Skeleton One Leg` and `Skeleton No Legs` have **zero MonoBehaviours and no NavMeshAgent**. They'd
spawn broken, which is why they're at `spawnWeight 0`. They need the full component stack
(`EnemyStateMachine`, `EnemyNav`, `EnemyHealth`, `EnemyAttack`, `EnemyAnimator`, `EnemyRagdoll`,
`EnemyDrop`, `BillBoard`, NavMeshAgent, Collider) plus a ragdoll rig before you raise the weight.
This is prefab work, not code — it's the "regular crawler" still outstanding on the board.

---

## Never verified in-editor

Everything below was written but not confirmed by playtest. **Nothing after the telegraph has been
run at all**, because the pool wiring blocks it.

- **Does the interception AI actually stop you outrunning enemies?** This is the entire Phase 1
  diagnosis and it's still unconfirmed on its own terms. Enemies were reaching and hitting you, which
  is indirect evidence, but nobody has deliberately tested pure fleeing.
- Do you take exactly 3 hits to die, with the vignette stepping mild → full tunnel → dead?
- Does the whole multi-type spawn system work at all — types appearing at the right waves, returning
  to the right pools, no leak over many waves?
- **Sprinter Crawler overrides its agent to 0.4 radius / 1.2 height.** If it falls through floors or
  clips geometry, that's why.

### Gotcha worth remembering
Enemy speed now comes from the **asset**, not the prefab. `ApplyMovement` overwrites the
NavMeshAgent every spawn. If enemies feel wrong, change **Move Speed on the asset** — editing the
prefab will appear to do nothing.

---

## What's next

Per `marrow-build-board.md`, Phase 1 has **~9 items left**. In rough priority:

1. **Finish the setup above and playtest.** Everything else is guesswork until the roster runs.
2. **Build out the two crawler prefabs** to close the Crawler board item.
3. **Grab a wind-up sound** (Freesound / Pixabay) and drop it on `EnemyTelegraph.windupClip` — that
   completes the telegraph's audible half with zero code, and "track the game by ear" is a core
   design pillar.
4. **Two ⛔ decisions are still yours to make**: target run length + difficulty ramp steepness, and
   picking the ONE wonder-weapon effect. Both gate other work.

**Phase 0 bugs were explicitly deprioritised** at your request — wall clipping, out-of-bounds
colliders, startup hitch. Worth noting the wall-clipping one undercuts the interception work, since
enemies phasing through geometry makes cut-off pathing meaningless.
