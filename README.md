# Project: Soul-like Vertical Slice

This repository contains the vertical slice for a Soul-like action RPG developed in Unity. The primary focus of this project was not just to create a playable demo, but to build it upon a robust, scalable, and maintainable software architecture. The design is heavily inspired by professional game development practices, emphasizing flexibility and separation of concerns.

**Engine:** Unity 2022.3.6f2
**Render Pipeline:** Universal Render Pipeline (URP) 

## Key Features

*   **Dynamic Player Controller:** A state-driven character controller allowing for fluid movement, combat (light/heavy attacks), dodging with I-frames, and hit reactions.
*   **Challenging Boss AI:** A multi-phased boss with distinct behaviors driven by a State Machine (`Idle`, `Chase`, `Attack`). The boss's aggression and speed increase in its second phase.
*   **Data-Driven Status System:** Character stats (Health, Stamina, Attack, Defense) are managed through Scriptable Objects, allowing for easy balancing and character variation without changing code.
*   **Player Progression System:** A fully functional Skill Tree that allows players to spend points to unlock permanent stat upgrades, influencing their playstyle.
*   **Event-Driven UI System:** A decoupled UI that reacts to game events, including a player HUD, a dynamic boss health bar, a skill tree window, and comprehensive game state screens (Pause, Victory, Game Over).
*   **Rich Game Feel & Polish:** The core combat loop is enhanced with various feedback mechanisms, including camera shake, hit VFX, multiple sound effects (hit, swing, dodge, footsteps), and UI feedback on damage.

---

## Architectural Deep Dive

The core philosophy of this project is **Separation of Concerns**. Instead of monolithic scripts, the architecture is composed of specialized systems that communicate efficiently through established design patterns.

### 1. Data-Oriented Design (with Scriptable Objects)

This is the foundational pillar of the project's flexibility.

*   **What it is:** I separate the "data" (e.g., how much health a character has) from the "behavior" (e.g., the code that makes a character take damage).
*   **How it's used:** `CharacterStats_SO` and `Skill_SO` act as "data blueprints." They are asset files that hold all the configurable variables for characters and skills.


### 2. State Pattern

This pattern is used to manage the complex behaviors of our dynamic characters.

*   **What it is:** An object's behavior is encapsulated within a family of "State" objects. The main object (the "Context") delegates its behavior to its current State object.
*   **How it's used:** `PlayerController` and `BossAIController` act as the Contexts. Their behavior at any given moment is determined by their current state (e.g., `PlayerGroundedState`, `ChaseState`). When an action occurs (like pressing a button or an enemy getting too close), the Context transitions to a new State.

### 3. Observer Pattern (Event-Driven Architecture)

This is the central nervous system of our project, enabling complete decoupling between major systems.

*   **What it is:** A "Subject" object maintains a list of its "Observers" and notifies them automatically of any state changes. We implement this using Scriptable Objects to create a global event bus.
*   **How it's used:** `GameEvent` assets act as broadcast channels. When `PlayerStatus` takes damage, it doesn't know about the UI; it simply `Raises` the `OnPlayerHealthChanged` event. The `UIManager` and `DamageVignetteUI` use a `GameEventListener` component to "listen" for this specific event and react accordingly.

### 4. Singleton Pattern

This pattern is used sparingly for globally accessible manager systems.

*   **What it is:** A pattern that ensures a class only has one instance and provides a global point of access to it.
*   **How it's used:** `GameManager`, `GameUIManager`, and `InputManager` are Singletons. This allows any system, anywhere, to easily access them (e.g., `GameManager.Instance.EnterUIMode()`).

---

