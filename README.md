# Zombie Diner

CS 583 Game Programming, Fall 2026: 3D Game Project

## Group Members

- Nathan Nguyen
- Carter Voight
- Jared Lane
- Jarion Sarmiento
- Andrew Saluta

## Current Target

Zombie Diner is a single-player, wave-based horde defense shooter inspired by *Overcooked* and *Call of Duty: Zombies*. From a bird's-eye, third-person view, the player defends their kitchen from zombies while cooking and serving food to survivors. Completed dishes earn money, which the player spends on guns, ammo, and health between increasingly difficult waves.

### MVP goals

- **Level:** One kitchen with endless zombie waves
- **Cooking:** Three dishes of increasing difficulty, with infinite ingredients taken from boxes
- **Orders:** Survivors request dishes and pay money, sometimes with bonus items
- **Weapons:** A weak handgun that is always equipped, plus stronger guns that take both hands
- **Shop:** An intermission between waves to buy guns, ammo, and health
- **Zombies:** Standard melee, food-eating (vegetarian), and exploding zombies
- **Controls:** Controller and keyboard and mouse support

We are still deciding how wave timers, revenue quotas, and failure conditions will work. The target may change as development continues.

### Working so far

- Player movement and aiming
- Temporary test gun that damages zombies
- Melee, exploding, and food-eating zombie behavior
- Wave system that adds more zombies over time
- Player health, with health and wave displays on the HUD
- Food crate with an infinite supply of food
- Picking up, carrying, and dropping food, or placing it on a table
- Food left on tables or the floor can be targeted and eaten by food zombies

### Still in progress

- Multiple ingredients, cooking stations, recipes, and the order queue
- Final weapons, weapon pickups, and the shop
- Connecting the ammo and cash displays to gameplay
- Full controller support
- Kitchen level, cartoon-style art, animation, and audio

## Team Roles and Goals

| Member | Area | Work so far | General goals |
| --- | --- | --- | --- |
| Nathan Nguyen | Zombie AI | Basic zombie AI, wave system, aiming contributions, test shooting, and bug fixes | Improve zombie behavior, balance the waves, and continue fixing bugs |
| Carter Voight | Zombie AI and food | Exploding zombie, food-eating zombie, food crate, food carrying, and interactable tables | Expand zombie types and connect food to the cooking system |
| Jared Lane | Player movement and aiming | Player movement and aiming | Refine movement, add dashing, and support controllers |
| Jarion Sarmiento | Player movement and aiming | Player movement and aiming | Refine aiming and support controllers |
| Andrew Saluta | UI | HUD for health, waves, ammo, and cash | Connect the remaining HUD elements and build order and shop UI |

## Controls

### Current

| Action | Keyboard and mouse |
| --- | --- |
| Move | WASD or arrow keys |
| Aim | Hold right mouse button and move the mouse |
| Shoot | Left mouse button |
| Pick up, place, or drop food | E |

### Planned

| Action | Controller | Keyboard and mouse |
| --- | --- | --- |
| Move | Left stick | WASD |
| Aim | Right stick | Hold right mouse button |
| Shoot | To be decided | Left mouse button |
| Pick up item | A | Space |
| Interact | X | E |
| Dash | Left bumper | Shift |