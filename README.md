# Robotron
Robocode for Picanhas Competition 2021!

Changelog
---------

- v0.1
  - Created discovery radar (turns all the time)
  - Tracked all enemies
  - Point gun at closest enemy and fire
- v0.2.1
  - Implemented anti-gravity movement (not working well)
- v0.2.2
  - Fixed coordinate system to compass coordinate system
  - Added log system
  - Antigravity works better now
- v0.3
  - Added rotation around the enemy when attacking
- v0.4
  - Tried to implement tracking enemies with a distance threshold
     but it didn't improve
- v0.5
  - Ramming for finishing robots
- v0.6
  - Avoid walls when rotating around an enemy
  - Approach enemy again if it is too far away
- v0.7
  - Team members go to their quadrants at the beginning
  - Team members notify each other of scanned robots
- v0.8
  - Added tracking score formula
  - Added strategy configuration
- v0.9
  - Notify other members of currently tracked enemy and
     when we stop tracking it to avoid tracking the same
- v1.0
  - Added radar locking when tracking an enemy
- v1.1
  - Tweaked radar locking when tracking an enemy
  - Refactored the state machines to make them more independant
- v1.2
  - Added predictive enemy tracking
- v1.3
  - Added win dance
  - Config for safe distance
  - Fixed a bug in radar locking
- 1.4
  - If only one enemy is left, all teammates go for it
  - Fixed a Fire() call that was left, changed to SetFire
  - Removed specific ClearFlag and cleared all flags before Execute()
  - Bullet hits are tracked to avoid ally fire on teammates
