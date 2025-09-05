===============
TextSpire Game
===============
Real-time, console-based deck-builder inspired by *Slay the Spire*.
Survive as many floors as you can with a 1 autosaving life!

-------------------
1.  QUICK START
-------------------
1. Run program.cs
2. Go to settings to customize the parameters
3. Start playing with these commands
   play N   draw next card and target enemy N (1 = first slime, etc.)
   use      drink first potion in your bag
   status   show HP / deck size / inventory
   quit     end run
   help     list commands again

4. After each victory pick 1 of 3 rooms:

      • Monster   – enter another fight.
      • Rest           – heal 75 % missing HP + gain a rest potion
      • Offense    – +1 Strike card, +1 permanent Strike bonus, find a weapon (+1 dmg each)
      • Defense   – +1 Defend card, +1 permanent Defend bonus

   Room choice is final. there is no back-tracking.

5. The game autosaves before entering the chosen room. Next time you play, start the program and pick “Load save” to continue.

----------------
2.  GAME LOOP
----------------
(1) Start → (2) Combat → (3) Choose Room → repeat ... until death

-------------
3.  COMBAT
-------------
• Real-time – every slime attacks in its own thread
  (default 2 s, configurable in Settings).
• You type commands while the slimes are hitting you.
• Deck = draw pile + discard pile. When the draw pile is empty the discard pile is shuffled back automatically.
• “Strike” deals 6 + Strike bonus + weapon bonus damage.
  “Defend” heals 5 + Defend bonus HP.

--------------------------------------------------------------------
4.  INVENTORY
--------------------------------------------------------------------
WEAPONS  – permanent; each adds +1 to every future Strike.
POTIONS  – one-use items:
                     – Rest Potion    (heals half of missing HP)
                     Potions follow FIFO order.