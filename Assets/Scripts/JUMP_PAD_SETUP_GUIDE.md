# Jump Pad Setup Guide

This guide will walk you through setting up a jump pad in your Unity scene that will launch your character upward when they step on it.

## Prerequisites

- Your player character must have:
  - `CharacterController` component
  - `ThirdPersonController` script attached
- Unity project with the `JumpPad.cs` script

---

## Step-by-Step Setup

### Step 1: Create the Jump Pad GameObject

1. **In the Unity Hierarchy**, right-click and select:
   - `3D Object → Cube` (or any 3D shape you prefer)
   
2. **Rename** the GameObject to `JumpPad`

3. **Position and scale** the jump pad:
   - Set the **Scale** to something flat like `(2, 0.2, 2)` to make it look like a platform
   - Position it where you want players to bounce from

---

### Step 2: Add the JumpPad Script

1. **Select** the JumpPad GameObject in the Hierarchy

2. **In the Inspector**, click `Add Component`

3. **Search** for `JumpPad` and select it

4. The script should now be attached to your GameObject

---

### Step 3: Configure the Collider (CRITICAL!)

This is the most important step! Without a proper collider, the jump pad won't detect the player.

#### Option A: Using a Trigger Collider (RECOMMENDED)

1. **Select** the JumpPad GameObject

2. **Check if it has a collider**:
   - If you created a Cube, it should already have a `Box Collider`
   - If not, click `Add Component` → search for `Box Collider`

3. **Enable "Is Trigger"**:
   - In the Box Collider component, **check the box** next to `Is Trigger`
   - This allows the player to pass through while still detecting collision

4. **Adjust the collider size**:
   - Make sure the collider covers the entire platform
   - You can click `Edit Collider` to adjust it visually

#### Option B: Using a Solid Collider

1. Follow steps 1-2 from Option A

2. **Leave "Is Trigger" unchecked**

3. **Note**: With this option, the player will physically collide with the pad (might feel less smooth)

---

### Step 4: Configure Jump Pad Settings

Select the JumpPad GameObject and adjust these settings in the Inspector:

#### Jump Settings

| Setting | Description | Recommended Value |
|---------|-------------|-------------------|
| **Jump Force** | How high the player bounces | `20` (adjust to taste) |
| **Horizontal Boost** | Optional sideways push | `(0, 0, 0)` for straight up |
| **Override Velocity** | Replace current velocity vs add to it | `✓ Checked` (recommended) |
| **Reset Jump Count** | Allow multi-jump after bounce | `✓ Checked` (recommended) |

#### Cooldown Settings

| Setting | Description | Recommended Value |
|---------|-------------|-------------------|
| **Cooldown Time** | Seconds before pad can be used again | `0.5` |

#### Visual Feedback

| Setting | Description | Recommended Value |
|---------|-------------|-------------------|
| **Animate On Bounce** | Compress/expand animation | `✓ Checked` |
| **Compression Amount** | How much it squishes (0-1) | `0.3` |
| **Animation Speed** | Speed of animation | `10` |
| **Change Color On Bounce** | Color feedback | `✓ Checked` |

---

### Step 5: Add Visual Polish (Optional)

#### Add a Material

1. **Create a new Material**:
   - Right-click in Project → `Create → Material`
   - Name it `JumpPadMaterial`

2. **Set the color**:
   - Select the material
   - Change the `Albedo` color to something bright (green, yellow, etc.)

3. **Apply to Jump Pad**:
   - Drag the material onto the JumpPad GameObject

#### Add Particle Effects (Optional)

1. **Create a particle system**:
   - Right-click the JumpPad in Hierarchy → `Effects → Particle System`
   - Rename it to `BounceEffect`

2. **Configure the particles**:
   - Set `Looping` to `OFF`
   - Set `Play On Awake` to `OFF`
   - Adjust emission, shape, and color to your liking

3. **Assign to Jump Pad**:
   - Select the JumpPad GameObject
   - In the JumpPad script component, drag the `BounceEffect` into the `Bounce Effect` slot

#### Add Sound Effects (Optional)

1. **Import a sound file** (bounce/spring sound)

2. **Assign to Jump Pad**:
   - Select the JumpPad GameObject
   - In the JumpPad script, drag your audio clip into the `Bounce Sound` slot

---

### Step 6: Test the Jump Pad

1. **Press Play** in Unity

2. **Move your character** onto the jump pad

3. **Check the Console** (Window → General → Console):
   - You should see messages like:
     ```
     [JumpPad] OnTriggerEnter detected: Player
     [JumpPad] TryBounce called for: Player
     [JumpPad] CharacterController found: True
     [JumpPad] ThirdPersonController found: True
     [JumpPad] Applying bounce with force: 20
     ```

4. **Your character should launch upward!**

---

## Troubleshooting

### Problem: Nothing happens when I step on the pad

**Solution 1: Check Collider**
- Make sure the JumpPad has a collider component
- Make sure "Is Trigger" is checked (for trigger-based detection)
- The collider should be large enough to cover the platform

**Solution 2: Check Layers**
- Make sure the player and jump pad aren't on layers that ignore each other
- Go to `Edit → Project Settings → Physics`
- Check the Layer Collision Matrix

**Solution 3: Check Console Messages**
- Open the Console (Window → General → Console)
- Look for `[JumpPad]` messages
- If you see "CharacterController found: False", the player doesn't have a CharacterController
- If you see "ThirdPersonController found: False", the player doesn't have the ThirdPersonController script

### Problem: The pad works but the bounce is too weak/strong

**Solution:**
- Select the JumpPad GameObject
- Adjust the `Jump Force` value in the Inspector
- Higher = stronger bounce
- Try values between 15-30

### Problem: The player bounces repeatedly

**Solution:**
- Increase the `Cooldown Time` to prevent rapid re-triggering
- Try setting it to `1.0` second

### Problem: I can't see the collision happening

**Solution:**
- Select the JumpPad in the Scene view
- You should see a yellow arrow showing the bounce direction (Gizmo)
- If the collider is a trigger, it will show as a green wireframe
- Make sure the collider covers the entire platform surface

---

## Advanced Configuration

### Creating Angled Jump Pads

To make the player bounce at an angle:

1. Set `Horizontal Boost` to a direction like `(5, 0, 0)` for a rightward boost
2. The player will bounce up AND to the right

### Creating Multiple Jump Pads

1. **Duplicate** the JumpPad GameObject (Ctrl+D)
2. **Position** it in a new location
3. Each pad can have **different settings** (different jump forces, directions, etc.)

### Creating a Jump Pad Chain

1. Create multiple jump pads at different heights
2. Set `Reset Jump Count` to `✓ Checked` on all of them
3. Players can chain bounces to reach very high places!

---

## Quick Checklist

Before asking for help, verify:

- [ ] JumpPad GameObject has the `JumpPad.cs` script attached
- [ ] JumpPad GameObject has a **Collider** component
- [ ] Collider has **"Is Trigger"** checked
- [ ] Player has **CharacterController** component
- [ ] Player has **ThirdPersonController** script
- [ ] Jump Force is set to a reasonable value (15-30)
- [ ] Console shows `[JumpPad]` messages when stepping on the pad

---

## Example Settings for Different Jump Types

### Small Hop (for minor elevation changes)
- Jump Force: `10`
- Horizontal Boost: `(0, 0, 0)`
- Cooldown: `0.3`

### Standard Jump (default)
- Jump Force: `20`
- Horizontal Boost: `(0, 0, 0)`
- Cooldown: `0.5`

### Super Jump (for reaching high places)
- Jump Force: `35`
- Horizontal Boost: `(0, 0, 0)`
- Cooldown: `1.0`

### Launch Pad (horizontal + vertical)
- Jump Force: `20`
- Horizontal Boost: `(10, 0, 0)` (adjust direction as needed)
- Cooldown: `0.5`

---

## Need More Help?

If the jump pad still isn't working:

1. **Check the Console** for `[JumpPad]` debug messages
2. **Take a screenshot** of:
   - The JumpPad Inspector settings
   - The Player Inspector (showing CharacterController and ThirdPersonController)
   - The Console messages
3. **Share the debug output** to get specific help

---

Good luck with your vertical platforming game! 🚀
