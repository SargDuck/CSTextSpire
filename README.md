# CSTextSpire

A real-time, console-based deck-builder inspire by Slay the Spire. Survive as many floors as you can you a single, autosaving life!

---

## User Documentation

### Quick Start
1. Run `Program.cs` or `dotnet run` from the project root.
2. Open settings to customize parameters (enemy HP, attack delay, etc.)
3. Start playing with these commands:
   ```
   play N   draw next card and target enemy N (1 = first slime, etc.)
   use      drink first potion in your bag
   status   show HP / deck size / inventory
   quit     end run
   help     list commands again
   ```
5. After each victory, pick 1 of 3 rooms:
- **Monster** – another fight
- **Rest** – heal 75% of missing HP + gain a Rest Potion
- **Offense** – +1 Strike card, +1 permanent Strike bonus, find a weapon (+1 dmg each)
- **Defense** – +1 Defend card, +1 permanent Defend bonus
- **Shop** – spend gold on items
- **Event** – random outcome (good or bad)

Room choice is final. there is no back-tracking.

5. The game autosaves before entering the chosen room.  
   Next time you play, pick **Load Save** to continue your run.

---

### Game Loop
(1) Start → (2) Combat → (3) Choose Room → repeat ... until death
- Each floor increases difficulty.
- The game ends when your HP drops to 0.

### Combat
- Real-time – each enemy attacks in its own thread (default 2s, configurable in Settings).
- You type commands while enemies attack.
- Deck = draw pile + discard pile. When the draw pile is empty, the discard pile is shuffled back automatically.
- **Cards:**
  - *Strike* – deals `6 + Strike bonus + weapon bonus` damage.
  - *Defend* – grants `6 + Defense bonus` temporary armor (absorbs damage before HP).
  - Other cards: Bash (adds Vulnerable), Cleave (splash damage), Reinforce (bigger armor), Shock (applies Weak).

### Inventory
- **Weapons** – permanent. Each adds +1 to every future Strike.
- **Potions** – one-use, FIFO order:
  - *Rest Potion*: heals half missing HP
  - *Heal Potion*: heals a fixed amount
- More potion/weapon types can be added easily.

# Developer Documentation

###
```
TextSpireCS/
├── Economy/
│ └── Wallet.cs -> Gold management
├── Engine/
│ ├── CombatRunner.cs -> Orchestrates combat loop and input
│ └── EnemyFactory.cs -> Builds enemy rosters
├── IO/
│ └── CommandParser.cs -> Parses console input
├── Model/
│ ├── Cards/
│ │ ├── Card.cs     -> Base card
│ │ ├── CardPool.cs -> Card templates
│ │ └── Deck.cs     -> Draw/discard piles
│ ├── Creature/
│ │ ├── Player.cs       -> The hero
│ │ ├── Enemy.cs        -> Enemy logic
│ │ ├── Hero.cs         -> Creates heroes
│ │ └── EnemyFactory.cs -> Creates enemies
│ ├── Item/
│ │ ├── Inventory.cs
│ │ ├── Potion.cs
│ │ └── Weapon.cs
│ ├── Misc/
│ │ ├── Achievements -> Run wide achievements 
│ │ ├── Mutators     -> Run wide effects
│ │ └── Relic        -> Run wide effects
│ └── World/
│   ├── Room.cs      -> Room type enum
│   └── RoomPicker.cs
├── Persist/
│ ├── RunHistory.cs  -> Run history entry
│ └── SaveGame.cs    -> Serializable save model
├── UI/
│ ├── AchievementsScreen.cs
│ ├── banner.txt
│ ├── BannerPrinter.cs
│ ├── CardRewardScreen.cs
│ ├── EventRoom.cs
│ ├── HeroSelectScreen.cs
│ ├── ManualScreen.cs
│ ├── ShopScreen.cs
│ ├── StatusPrinter.cs
│ └── RunHistoryScreen.cs
├── Util/
│ ├── Rng.cs
│ ├── ThreadUtils.cs
│ └── StatusPrinter.cs
└── Program.cs -> Entry point, main loop
```
---

### Class Responsibilities

#### Economy
- **Wallet:** Tracks and manages player gold across the run. Supports spending, earning, and persistence.

#### Engine
- **CombatContext:** Shared state of a combat encounter (player, enemies, locks, tokens, status effects). Provides synchronization between threads.
- **CombatRunner:** Spawns enemy threads, processes keyboard input, dispatches commands (`play`, `use`, `status`, `quit`, `help`).

#### IO
- **CommandParser:** Reads and validates player commands (e.g., play N, use, status). Provides structured command parsing results.

#### Model
##### Cards
- **Card:** Represents a single card with a name and damage/armor value.
- **CardPool:** Stores a variety of cards.
- **Deck:** Manages the player’s draw/discard piles. Handles drawing, shuffling, and discarding mechanics.

##### Creature
- **Enemy:** Represents enemy stats, attack loop, and damage-taking logic.
- **EnemyFactory:** Scales enemies per floor and generates rosters.
- **Hero:** Defines hero classes and their base decks/HP.
- **Player:** Represents the human-controlled character with HP, armor, and deck.

##### Item
- **Inventory:** Stores and manages player potions and weapons.
- **Potion:** One-use item, heals HP when consumed.
- **Weapon:** Permanent item, grants ongoing damage bonuses.

#### Misc
- **Achievements:** Defines achievements.
- **Mutators:** Extra game modifiers.
- **Relic:** Passive permanent bonuses.

#### World
- **Room:** Stores RoomType (Monster, Rest, Offense, Defense, Shop, Event).
- **RoomPicker:** Randomly selects available rooms to present to the player.

#### Persist
- **RunHistory:** Records data for each completed run (floor reached, gold, class).
- **SaveGame:** Serializable representation of game state (deck, inventory, bonuses).

#### UI
- **AchievementsScreen:** Displays unlocked achievements.
- **BannerPrinter:** Prints ASCII banner art.
- **CardRewardScreen:** Presents reward choices after victories.
- **EventRoomScreen:** Handles interactive event rooms.
- **HeroSelectScreen:** Allows hero selection at the start.
- **ManualScreen:** Shows user instructions.
- **RunHistoryScreen:** Displays run history log.
- **ShopScreen:** Handles buying potions, weapons, and cards.
- **StatusPrinter:** Prints current combat and player status.

#### Util
- **Rng:** Provides a shared Random instance.
- **SaveManager:** Handles read/write of save files.
- **ThreadUtils:** Provides safe wrappers around thread sleeping.

#### Entry Point
- **Program.cs:** Main entry point. Prints banner, runs menu (New Game, Load Save, Settings, Manual, Achievements, Run History), sets up game loop, runs combat + rooms, and saves progress.

---

### Concept Mapping

| Game Concept        | Class/Component |
| ------------------- | ----------------|
| Player              | `Player`, `Deck`, `Inventory` |
| Heroes (classes)    | `Hero`, `HeroSelectScreen`|
| Enemies             | `Enemy`, `EnemyFactory` |
| Cards               | `Card`, `Deck`, `CardPool` |
| Inventory Items     | `Potion`, `Weapon`, `Inventory` |
| Gold / Currency     | `Wallet` |
| Rooms               | `Room`, `RoomPicker` |
| Shops               | `ShopScreen` |
| Events              | `EventRoomScreen` |
| Achievements        | `Achievements`, `AchievementsScreen` |
| Combat Engine       | `CombatContext`, `CombatRunner`, `CommandParser` |
| Save System         | `SaveManager`, `SaveGame`, `RunHistory`, `RunHistoryScreen` |
| UI & Menus          | `BannerPrinter`, `ManualScreen`, `HeroSelectScreen`, `CardRewardScreen` |
| Game Loop (entry)   | `Program.cs` |
| Utilities           | `Rng`, `ThreadUtils` |


---

### Notes
- **Thread safety:** all combat state changes (HP, armor, deck) are guarded by `ctx.Lock` inside `CombatRunner`.
- **Asynchronous combat:** each enemy runs as a separate `Task`, with cancellation via `ctx.Token`.
- **saves:** writes go to `save.json.tmp` then are replaced in one step.
- **Extensibility:** new cards, rooms, items, or events can be added by extending enums and updating dispatch methods.

---
