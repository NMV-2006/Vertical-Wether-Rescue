# Push Zone Setup Guide

This guide will show you how to create zones that push the player in any direction - perfect for wind tunnels, updrafts, geysers, or any pushing mechanic!

## What is a Push Zone?

A **Push Zone** is an invisible area that continuously applies force to the player while they're inside it. Unlike the Jump Pad (which gives one big push), the Push Zone keeps pushing as long as you're in it.

**Use cases:**
- 🌪️ Wind tunnels that push you horizontally
- ⬆️ Updrafts that lift you vertically
- 🌊 Water currents
- 💨 Gusts of wind
- 🚀 Launch tubes

---

## Quick Setup (5 Steps)

### Step 1: Create the Zone GameObject

1. **In Unity Hierarchy**, right-click and select:
   - `Create Empty` (or `3D Object → Cube` if you want to see it)

2. **Rename** it to `PushZone` or `WindZone`

3. **Position** it where you want the push zone to be

---

### Step 2: Add a Trigger Collider

**CRITICAL:** The zone needs a trigger collider to detect the player!

1. **Select** the PushZone GameObject

2. **Add a collider**:
   - Click `Add Component`
   - Search for `Box Collider` (or Sphere/Capsule)
   - Click to add it

3. **Make it a trigger**:
   - In the collider component, **check "Is Trigger"**

4. **Adjust the size**:
   - Set the `Size` to cover the area you want (e.g., `10, 10, 10` for a large zone)
   - You can click `Edit Collider` to adjust it visually in the scene

---

### Step 3: Add the PushZone Script

1. **Select** the PushZone GameObject

2. **Click** `Add Component`

3. **Search** for `PushZone` and select it

---

### Step 4: Configure the Push Force

This is where you set the direction and strength!

1. **Select** the PushZone GameObject

2. **In the Inspector**, find the `Push Force` setting

3. **Set the direction**:
   - `(0, 10, 0)` = Push **upward** (updraft)
   - `(10, 0, 0)` = Push **right** (horizontal wind)
   - `(0, 0, 10)` = Push **forward** (wind tunnel)
   - `(-5, 5, 0)` = Push **up and left** (diagonal)

4. **Adjust the strength**:
   - Higher numbers = stronger push
   - Start with `10` and adjust from there

---

### Step 5: Test It!

1. **Press Play**

2. **Walk into the zone**

3. **You should be pushed** in the direction you set!

4. **Check the Console** for debug messages like:
   ```
   [PushZone] Player entered zone! Push force: (0, 10, 0)
   ```

---

## Common Zone Types

### Updraft (Vertical Wind)

Perfect for lifting the player to higher areas.

**Settings:**
- Push Force: `(0, 15, 0)` (upward)
- Continuous Force: `✓ Checked`
- Override Velocity: `☐ Unchecked` (adds to current velocity)
- Accelerate Over Time: `☐ Unchecked`

**Collider:**
- Box Collider: `Size (5, 20, 5)` (tall cylinder shape)

---

### Wind Tunnel (Horizontal Push)

Push the player through a corridor or across a gap.

**Settings:**
- Push Force: `(20, 0, 0)` (adjust direction as needed)
- Continuous Force: `✓ Checked`
- Override Velocity: `☐ Unchecked`
- Control Multiplier: `0.3` (reduce player control)

**Collider:**
- Box Collider: `Size (20, 5, 5)` (long tunnel shape)

---

### Geyser (Burst Upward)

Strong upward force that launches you high.

**Settings:**
- Push Force: `(0, 30, 0)` (strong upward)
- Continuous Force: `✓ Checked`
- Override Velocity: `✓ Checked` (replace velocity)
- Accelerate Over Time: `✓ Checked`
- Max Force Multiplier: `2`
- Acceleration Time: `1` second

**Collider:**
- Cylinder Collider or Box: `Size (3, 15, 3)`

---

### Gentle Breeze (Subtle Push)

Slight push that guides the player.

**Settings:**
- Push Force: `(3, 0, 0)` (gentle)
- Continuous Force: `✓ Checked`
- Override Velocity: `☐ Unchecked`
- Control Multiplier: `0.8` (mostly keep control)

---

## Advanced Settings Explained

### Push Settings

| Setting | Description | Example |
|---------|-------------|---------|
| **Push Force** | Direction (X,Y,Z) and strength | `(0, 10, 0)` |
| **Force Mode** | How force is applied | `Acceleration` (default) |
| **Accelerate Over Time** | Force gets stronger the longer you're in it | `☐` for constant |
| **Max Force Multiplier** | How much stronger it gets | `3` = 3x stronger |
| **Acceleration Time** | Seconds to reach max force | `2` seconds |

### Force Application

| Setting | Description | Recommended |
|---------|-------------|-------------|
| **Override Velocity** | Replace current velocity vs add to it | `☐` (add to it) |
| **Continuous Force** | Apply every frame (smooth) | `✓` Checked |
| **Force Interval** | If not continuous, how often to push | `0.1` seconds |

### Player Control

| Setting | Description | Recommended |
|---------|-------------|-------------|
| **Disable Player Control** | Completely disable movement | Usually `☐` |
| **Control Multiplier** | Reduce control (0-1) | `0.5` (half control) |
| **Reset Jump On Enter** | Allow multi-jump when entering | `✓` Checked |

---

## Adding Visual Effects

### Add Particle Effects (Wind Visual)

1. **Right-click** the PushZone in Hierarchy
   - `Effects → Particle System`

2. **Configure the particles**:
   - Set `Looping` to `ON`
   - Set `Start Speed` to match your push force direction
   - Set `Gravity Modifier` to `0`
   - Adjust `Shape` to match your zone (Box, Sphere, etc.)

3. **Assign to PushZone**:
   - Select the PushZone GameObject
   - Drag the Particle System into the `Wind Effect` slot

4. **Optional**: Check `Particles Only When Occupied` to save performance

---

### Add Sound Effects (Wind Audio)

1. **Add an Audio Source**:
   - Select PushZone
   - `Add Component → Audio Source`

2. **Configure it**:
   - Assign a wind sound clip
   - Set `Loop` to `ON`
   - Set `Play On Awake` to `ON`
   - Adjust `Volume` to taste

3. **Assign to PushZone**:
   - In the PushZone script, drag the Audio Source into the `Wind Sound` slot

4. **Enable fading**:
   - Check `Fade Audio` to make it fade in/out when player enters/exits

---

## Troubleshooting

### Problem: Nothing happens when I enter the zone

**Solutions:**

1. **Check the collider**:
   - Make sure the zone has a collider
   - Make sure "Is Trigger" is checked
   - Make sure the collider is big enough

2. **Check the Console**:
   - You should see `[PushZone] Player entered zone!`
   - If you don't see this, the player isn't being detected

3. **Check the player**:
   - Player must have `ThirdPersonController` script
   - Player must have `CharacterController` component

---

### Problem: The push is too weak/strong

**Solution:**
- Adjust the `Push Force` values
- Higher numbers = stronger push
- Try values between 5-30 depending on the effect you want

---

### Problem: Player gets stuck in the zone

**Solutions:**

1. **Reduce Override Velocity**:
   - Uncheck `Override Velocity` to add force instead of replacing it

2. **Reduce Control Multiplier**:
   - Set `Control Multiplier` to `0.7` or higher to give more control

3. **Make the zone bigger**:
   - Give the player more room to move around

---

### Problem: The force is jittery/inconsistent

**Solution:**
- Make sure `Continuous Force` is checked
- This applies force every frame for smooth movement

---

## Creative Ideas

### 1. **Vertical Platforming Challenge**
Create a series of updrafts that the player must navigate through to climb a tower.

### 2. **Wind Maze**
Create a maze where different zones push you in different directions - you must time your movement!

### 3. **Launch Tube System**
Create tubes that push you from one area to another (like Mario Galaxy).

### 4. **Tornado Effect**
Create multiple zones in a spiral pattern, each pushing in a slightly different direction.

### 5. **Floating Islands**
Use gentle updrafts around islands to help the player stay airborne longer.

---

## Combining with Other Mechanics

### Push Zone + Jump Pad
- Place a jump pad at the bottom of an updraft
- Jump pad launches you up, updraft keeps you floating

### Push Zone + Gliding
- Use horizontal push zones while gliding
- Creates dynamic aerial navigation

### Push Zone + Moving Platforms
- Add push zones to moving platforms
- Creates challenging timing puzzles

---

## Quick Reference: Common Force Values

| Effect | Push Force | Notes |
|--------|------------|-------|
| Gentle breeze | `(3, 0, 0)` | Subtle guidance |
| Standard wind | `(10, 0, 0)` | Noticeable push |
| Strong wind | `(20, 0, 0)` | Hard to fight |
| Updraft | `(0, 15, 0)` | Lifts player up |
| Geyser | `(0, 30, 0)` | Strong launch |
| Diagonal wind | `(10, 5, 0)` | Up and sideways |

---

## Debug Tips

1. **Enable Debug Logs**:
   - Check `Show Debug Logs` in the PushZone script
   - Watch the Console for force application messages

2. **Show Gizmos**:
   - Check `Show Gizmos` to see the force direction in the Scene view
   - Cyan arrows show which way the force pushes

3. **Test in Scene View**:
   - Select the PushZone
   - You'll see the zone bounds and force arrows
   - Make sure the arrows point the right direction!

---

## Checklist Before Testing

- [ ] PushZone has a **Collider** component
- [ ] Collider has **"Is Trigger"** checked
- [ ] **Push Force** is set to desired direction and strength
- [ ] **Continuous Force** is checked (for smooth pushing)
- [ ] Player has **ThirdPersonController** script
- [ ] **Show Debug Logs** is enabled (for troubleshooting)

---

Good luck creating dynamic push zones! 🌪️💨
