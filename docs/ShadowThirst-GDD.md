# GAME DESIGN DOCUMENT (GDD)

## 1. Game Overview

**Game Title:** Shadow Thirst  
**Platform:** Android  
**Engine:** Unity 2D (URP)  
**Genre:** Stealth / Survival (Endless, Score-Based)  
**Theme:** Avoid the Light  

**One-Line Description:**  
A top-down 2D stealth survival game where a vampire must survive in darkness, avoid deadly light hazards, manage blood thirst, and deal with patrolling enemies while scoring as high as possible.

---

## 2. Core Idea

The player controls a vampire who cannot survive exposure to light.  
The environment is filled with light hazards and patrolling enemies that apply constant pressure.

Survival depends on:
- Reading light patterns  
- Avoiding or surviving enemy encounters  
- Managing a draining blood meter  
- Using temporary abilities wisely  

Light remains the **primary threat**, while enemies act as **secondary pressure units**.

---

## 3. Core Gameplay Loop (Endless)

Start Game  
→ Observe light hazards and enemy patrols  
→ Move through shadowed areas  
→ Avoid light or activate Shadow Shield  
→ Avoid enemies or take damage  
→ Collect vials  
→ Blood meter refills / effects applied  
→ Score increases over time  
→ Difficulty pressure increases  

→ Touch light without active shield  
OR Blood meter reaches zero  
→ Game Over  
→ Show final score  
→ Restart  

---

## 4. Player Mechanics

- Top-down 2D movement via on-screen joystick  
- Free movement in all directions  
- Player dies instantly when touching light without an active Shadow Shield  
- Blood meter drains continuously over time  
- Player can take damage from enemy attacks  
- Player stops movement while attacking  
- Player always faces the enemy during attack animations  
- Score increases with survival time and interactions  

---

## 5. Enemy System

### Enemy Type: Patrolling Shadow Hunter

**Behavior:**
- Patrols between two predefined points  
- Switches direction upon reaching patrol endpoints  
- Detects player within a trigger range  
- Stops patrolling when player in range  
- Attacks player at fixed intervals  
- Start patrolling once player move out of range  

**Combat Rules:**
- Enemy attacks reduce player blood/health  
- Enemy attacks can trigger player hurt animation  
- Enemy does NOT instantly kill the player  
- Enemy can be killed by player attack when in range  
- Enemy attacks reduce player points

Enemies act as **pressure threats**, forcing risky movement choices and interaction with light hazards.

---

## 6. Light Hazards

### Moving Lights
- Rotating spotlight-style lights  
- Linear moving beams  
- Predictable movement patterns  

### Stationary Lights
- Always-on lights blocking key paths  
- Require Shadow Shield to pass safely  

### Lighting Rules
- Light contact = instant death unless shield is active  
- Freeform Light2D used for accurate beam shapes  
- Reduced Global Light2D intensity for readability  

---

## 7. Pickups & Vials

### Shadow Shield Vial
- Only one shield can be held at a time  
- Manually activated via UI button  
- Grants temporary immunity to light  
- Shield button deactivates immediately after use  
- Shield duration shown via UI indicator  

### Blood Vial
- Restores player blood  
- Prevents death from blood depletion  
- Grants small score bonus  
- Static placement in level  

### Poison Vial
- Instantly damages the player  
- Reduces blood  
- Applies score penalty  
- High-risk pickup  

### Speed Boost Vial
- Temporarily increases player movement speed  
- Helps escape enemies or cross dangerous areas  
- Limited duration  
- No stacking  

---

## 8. Blood System

- Blood meter drains over time  
- Enemy attacks reduce blood  
- Blood vials restore blood  
- Poison vials reduce blood  
- Blood depletion results in death  
- Nearest blood vial direction indicator available  
- Indicator includes padding to avoid overlap  

---

## 9. Scoring System

- Score increases based on survival time  
- Bonus score for collecting blood vials  
- Bonus score for killing enemies  
- Score penalty for poison damage  
- Final score shown on Game Over screen  

---

## 10. Lose Conditions

- Touching light without active Shadow Shield  
- Blood meter reaches zero  

---

## 11. Controls (Android)

- Touch & Drag Joystick: Move  
- Attack Button: Attack enemy in range  
- Shield Button: Activate Shadow Shield  
- Pause Button: Pause game  
- UI Buttons: Restart / Quit  

---

## 12. Level & Camera

- Single fixed level  
- Fixed camera  
- Endless gameplay through increasing pressure  
- Static layout with dynamic threats  

---

## 13. Visual Style

- Top-down 2D  
- High contrast visuals  
- Shadows: Black  
- Lights: Yellow / warm tones  
- Blood: Red  
- Poison: Green  
- Speed Boost: Blue  
- Enemies: Dark silhouettes with readable animations  

---

## 14. Scope & Constraints

- No procedural generation  
- No moving camera  
- No inventory system  
- No cutscenes  
- No online features  

---

## 15. Technical Notes

- Engine: Unity 2D (URP)  
- Physics: Rigidbody2D  
- Lighting: Light2D + Sprite beams  
- Hazards: Trigger-based colliders  
- Enemy AI: Patrol + range-based attack  
- UI: Unity Canvas  
- Audio: Minimal background music and essential SFX  

---
