GAME DESIGN DOCUMENT (GDD)

1. Game Overview

Game Title: Shadow Thirst
Platform: Android
Engine: Unity 2D
Genre: Stealth / Survival (Endless, Score-Based)
Theme Used: Avoid The Light

One-Line Description:
A top-down 2D stealth survival game where a vampire must stay in shadows, avoid light exposure, collect blood to survive, and score as high as possible.

2. Core Idea

The player controls a vampire who cannot survive in light and thrives only in darkness. The player must carefully move through shadowed areas, avoid being exposed to moving light, strategically use a time-limited Shadow Shield to survive unavoidable stationary lights, collect blood to stay alive, and survive for as long as possible to achieve a high score.

Light is an absolute threat and defines all gameplay decisions.

Optional (If Time Permits):
The environment may include limited movable objects that can be pushed to reposition predefined shadow zones, allowing the player to temporarily block or shorten long light paths. This mechanic is optional and not required for survival.

3. Core Gameplay Loop (Endless)

Start Game
→ Observe light patterns
→ Move through shadowed areas
→ Avoid or survive light exposure
→ Collect blood
→ Blood meter refills
→ Score increases with survival time
→ Difficulty increases over time
→ Survive as long as possible

→ Touch light without active shield OR Blood meter reaches zero
→ Game Over
→ Show final score
→ Restart

Note: There is no win condition.

4. Player Mechanics

- Top-down 2D movement
- Free movement inside shadow areas
- Instant death when touching light without an active Shadow Shield
- Blood meter slowly drains over time
- Blood pickups refill blood meter
- Score increases based on time survived
- Player can activate a time-limited Shadow Shield
- Optional: Player can push limited environmental objects
- No combat, no attacks, no enemies

5. Light Hazards

Moving Lights:
- Rotating spotlight-style lights (360° or limited arc)
- Patrolling cylindrical lights moving linearly
- Constant, predictable speed
- Always active

Stationary Lights:
- Always-ON lights (require Shadow Shield)
- ON/OFF lights with predictable cycles

Global Rule:
Contact with any light causes instant death unless Shadow Shield is active.

6. Power-Ups

Shadow Shield:
- Only one shield can be held at a time
- Manually activated
- Grants 3 seconds of light immunity
- No stacking or extension

Blood Surge:
- Instantly refills blood meter

7. Blood System

- Static blood pickups
- Predefined spawn points
- Randomized at level start
- No dynamic respawn

8. Scoring System

- Score increases with survival time
- Small bonus for collecting blood
- Final score shown on Game Over

9. Lose Conditions

- Touching light without active Shadow Shield
- Blood meter reaches zero

10. Controls (Android)

- Touch & Drag: Move
- On-screen Button: Activate Shadow Shield
- Optional: Push objects
- On-screen Button: Pause
- UI Buttons: Restart / Quit

11. Level & Camera

- Single fixed level
- Fixed camera
- Endless gameplay via increasing difficulty
- Optional push mechanics (if time permits)

12. Visual Style

- Top-down 2D
- High contrast
- Shadows: Black
- Lights: Yellow
- Blood: Red
- Vampire: Dark silhouette
- No real-time lighting

13. Scope & Constraints

- No combat
- No enemies or AI
- No real-time lighting
- No inventory
- No cutscenes
- No procedural generation
- No moving camera
- No online features

14. Technical Notes

- Engine: Unity 2D
- Physics: Rigidbody2D
- Lights: Sprite + Collider2D (Trigger)
- UI: Unity Canvas
- Audio: Minimal background music and SFX

