# Marrow — Build Board

> Playable, not yet fun. This board is ordered by **what makes the game fun**, not by what's easiest.
> Work top-to-bottom. Do not skip ahead to polish while Phase 1 is unchecked.
>
> **This file is the source of truth for "what's next."** CLAUDE.md points here — check the ▶ NEXT UP
> section below before picking up new work, and update this file (not just CLAUDE.md) when a task
> gets finished.

**Legend:** `[ ]` todo · `[x]` done · 🐛 bug/blocker · ⛔ DECISION — needs Matthew, do not decide alone · ▪ build task

---

## ▶ NEXT UP

1. ▪ **Enemy interception AI** (Phase 1). Still the single highest-leverage task on the board. Forward-biased spawning is live (`WaveManager.PickSpawnPoint`, `leadTime`/`aheadWeight`), but `ChaseState` still calls `EnemyNav.SetDestination(esm.PlayerTransform.position)` — plain chase-to-current-position, no predicted lead. Ambush spawns alone don't fix the outrun problem once the player evades the first contact; the chase itself needs to cut the player off.
2. 🐛 **Remaining Phase 0 blockers**: enemies/player clipping through walls, out-of-bounds colliders too short, startup framerate hitch. Quick, but still open — should close out before leaning further into Phase 1 build tasks.

If a task is marked ⛔, **stop and ask Matthew** — it's a design call, not an implementation detail.

---

## The diagnosis (why this order)

You outrun the enemies → you're never in danger → killing is optional → combat is pointless → the game is boring. Everything else (sound, UI, juice, a new map) *amplifies* the experience; it can't create one. **Fix threat first.** Polishing an unfun loop yields a beautiful game nobody wants to play.

---

## Phase 0 — Blockers & Bugs
Broken things that make everything downstream lie to you. Most are under an hour.

- [x] 🐛 Wire `RegisterKill()` into `EnemyHealth`'s death path — kills don't charge the shotgun at all right now
- [x] 🐛 Fix throwable aim — launches off to the side of the crosshair (throw along the camera ray, not the hand transform)
- [x] 🐛 Slow throwable velocity — currently thrown far too hard/fast
- [x] 🐛 Throwables pass through walls and get wasted (continuous collision detection on the rigidbody)
- [ ] 🐛 Enemies clip through walls they shouldn't (NavMesh carving / missing obstacle colliders)
- [ ] 🐛 Player passes through certain walls (collider gaps in the modular alleyway seams)
- [x] 🐛 Skeletons stop ragdolling after a while and vanish (ragdoll cap / pooling limit)
- [ ] 🐛 Raise out-of-bounds colliders so the boost can't launch the player out of the map
- [ ] 🐛 Framerate hitch at run start (likely instantiate spike — pool enemies/props on load)

## Phase 1 — Make It Fun: Threat & Combat
**This is the whole game.** Goal: make standing still lethal and make escape cost something.

- [x] ⛔ DECIDE: how does the game stop you simply outrunning everything? (enemy speed / sprinters / interceptors / stamina / tighter map — probably two of these, not all five)
- [x] ▪ Rebalance enemy vs. player speed so pure fleeing fails
- [ ] ▪ Smarter AI: intercept and cut off, don't just chase the transform (path to a predicted future position) — **verified still open, see NEXT UP**
- [x] ▪ Spawn enemies *ahead* of the player and around corners, not just behind — implemented in `WaveManager.cs` (`leadTime`, `aheadWeight`, `PickSpawnPoint()` weighted forward-cone pick)
- [ ] ▪ Enemy type: Sprinter (fast, fragile, punishes running)
- [ ] ▪ Enemy type: Crawler (low, hard to hit, clogs corridors)
- [ ] ▪ Enemy type: Brute/Blocker (slow, tanky, denies a route)
- [ ] ▪ Enemy attack telegraph — visible AND audible wind-up
- [ ] ▪ Make escape cost something (stamina, or slow while firing/throwing)
- [ ] ▪ Make killing mandatory, not optional (enemies block progress; shells/mobility come from kills)
- [ ] ▪ Add the "wonder" to the wonder weapon — pick ONE exotic effect (chain lightning / dismemberment / ragdoll-launch / freeze-shatter)
- [ ] ▪ Calibrate shotgun: spread, damage falloff, boost force (wider spread, more impulse)
- [ ] ▪ Make throwables feel good: arc, weight, satisfying impact
- [ ] ▪ Kills need weight: gibs, ragdoll launch, hitstop
- [ ] ⛔ DECIDE: target run length + difficulty ramp steepness (last run was 10 min — is that the target?)
- [ ] ▪ Wave scaling: count, speed, health, type mix per wave
- [ ] ▪ Difficulty meter / selector

## Phase 2 — Economy & Systems
Infinite points and a train that's already there is not a game. Tune against the Phase 1 run-length decision.

- [ ] ⛔ DECIDE: what earns points, and what the late-game sink is (so you're not idling rich)
- [ ] ▪ Replace infinite points with a real earn/spend economy
- [ ] ▪ Award points on kill / hit / wave clear
- [ ] ▪ Price doors, perks, and the train call so they compete for the same points
- [ ] ▪ Build the train CALL system — the train should NOT start on the map
- [ ] ▪ Train call cost + hold-out wave before it arrives (this is the ending)
- [ ] ▪ Build the market: buy throwables / shells with points (the late-game sink)
- [ ] ▪ Add more perks beyond Slide / Armor / Power Throw
- [ ] ⛔ DECIDE: what Trickster's "counters" actually does — or cut it

## Phase 3 — The Power Moment
Best idea in the audit. Power isn't a switch, it's a transformation. Build it as one orchestrated beat.

- [ ] ▪ Make the map genuinely dark and dead before power (lights off, heavy fog, no signage)
- [ ] ▪ Power-on: sky turns red
- [ ] ▪ Power-on: scenery change — lights snap on, signs glow, fog shifts colour
- [ ] ▪ Power-on stinger: sound + rumble + a beat of silence before the wave
- [ ] ▪ Power gates perks, market, and the train call — make the dependency real
- [ ] ▪ Consider: power turning on makes enemies HARDER (turn the reward into a bargain)

## Phase 4 — Legibility & UI
You couldn't tell your health, shells, power state, or objective. That's why the game feels formless.

- [ ] ⛔ DECIDE: commit to the ASCII UI direction (or don't) — it changes every element below
- [ ] ▪ Health feedback: COD-Zombies style blood/damage vignette — *temp debug version exists* (`DamageVignette.cs`: procedural red tunnel-vision overlay driven by `PlayerStats.OnDamaged`/`OnHealthChanged`). Built as a read-out for testing enemy damage; still needs the real art/shader pass before this ticks.
- [ ] ▪ Damage direction indicator (critical for the chill test)
- [ ] ▪ Shell counter (3 pips) + progress toward next shell
- [ ] ▪ Objective prompt: what am I supposed to do RIGHT NOW
- [ ] ▪ Power on/off state readable at a glance
- [ ] ▪ Train called / incoming indicator + countdown
- [ ] ▪ Interaction prompts on doors, perks, power, market, train
- [ ] ▪ Points readout + can-I-afford-this feedback on every buyable

## Phase 5 — Sound Pass
Zero sound exists. This is THE design pillar — track the game by ear. Mono for 3D-positional; seamless loops for ambience.

- [ ] ▪ Enemy approach / idle / growl cues (most important sounds in the game)
- [ ] ▪ Enemy attack telegraph sound
- [ ] ▪ Enemy death sounds
- [ ] ▪ Shotgun: fire, dry click, shell gained
- [ ] ▪ Boost whoosh + landing impact
- [ ] ▪ Throwable: throw, impact, shatter
- [ ] ▪ Hit confirm
- [ ] ▪ Player damage + low-health heartbeat
- [ ] ▪ Wave start / wave clear stingers
- [ ] ▪ Door, perk, market purchase sounds
- [ ] ▪ Train: call confirm, distant horn, arrival
- [ ] ▪ Ambient alley loop (different pre- and post-power)
- [ ] ▪ Tension layer that rises with the wave (optional)

## Phase 6 — Juice & Feel Pass
Your verdict: "none of these feel good." Every action should push back.

- [ ] ▪ Shotgun muzzle flash + particle effect
- [ ] ▪ Impact particles: blood, bone dust, wall chips
- [ ] ▪ Camera shake pass — fire, boost, land, take damage
- [ ] ▪ Hitstop on kills
- [ ] ▪ Train arrival: headlight sweep out of the fog, ground rumble
- [ ] ▪ Win / death transitions (reuse the existing fade-to-black)

## Phase 7 — Map & Variety
"Everything feels the same." Fix the map you *have* — a new one won't rescue an unfun loop.

- [ ] ▪ Break up the sameness: distinct areas with their own character and landmarks
- [ ] ▪ Fill dead space — set dressing, clutter, silhouettes
- [ ] ▪ Design routes *for* the boost — intentional rooftop paths instead of exploits
- [ ] ▪ Add chokepoints and arenas so fights have shape
- [ ] ▪ Turn the starting bar into a proper jazz bar with character
- [ ] ▪ Fix throwable scatter placing props in stupid spots (constrain NavMesh scatter to sensible surfaces)
- [ ] ▪ Lighting consistency + PS1 aesthetic check across every area

## Phase 8 — Playtest & Ship
Re-run the audit after Phase 1. If "is it fun" is still no, STOP and go back — don't proceed to polish.

- [ ] ▪ Re-run the playtest audit after the threat fix (the only question that matters: is combat fun yet?)
- [ ] ▪ Standalone build
- [ ] ▪ Discord playtest with friends
- [ ] ▪ The chill test: can players track the game by ear while chatting?
- [ ] ▪ Bug pass from playtest notes
- [ ] ▪ Final build + tag a release on GitHub

---

## Backlog — Deliberately Not Now
Good ideas, but a *different, larger game*. A better map amplifies fun; it can't create it. Park these until Marrow is fun on the map you already have.

- [ ] Skyscraper map: building-to-building above the clouds (perfect fit for the boost — but a whole new level)
- [ ] Storm rolls in when power turns on (skyscraper map)
- [ ] NPCs to talk to in the jazz bar (dialogue is an entire system you don't have yet)
