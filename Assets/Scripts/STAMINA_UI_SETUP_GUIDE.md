# Stamina Radial Meter UI - Setup Guide

## 📋 Quick Setup Instructions

### Step 1: Create the UI Canvas
1. In Unity Hierarchy, right-click → **UI** → **Canvas**
2. Set Canvas **Render Mode** to "Screen Space - Overlay"
3. Set Canvas Scaler **UI Scale Mode** to "Scale With Screen Size"
4. Set Reference Resolution to **1920 x 1080**

### Step 2: Create the Radial Meter
1. Right-click on Canvas → **UI** → **Image** (name it "StaminaMeter")
2. Position it in the corner (recommended: bottom-left or bottom-right)
   - Anchor: Bottom-Left or Bottom-Right
   - Position X: 100, Y: 100 (adjust as needed)
   - Width: 150, Height: 150

### Step 3: Create Background Circle (Optional)
1. Right-click on StaminaMeter → **UI** → **Image** (name it "Background")
2. Set as child of StaminaMeter
3. Set Anchor to **Stretch** (both horizontal and vertical)
4. Set all offsets to 0
5. Set color to dark gray/black with some transparency
   - Example: R:0, G:0, B:0, A:100

### Step 4: Create the Radial Fill
1. Right-click on StaminaMeter → **UI** → **Image** (name it "RadialFill")
2. Set as child of StaminaMeter
3. Set Anchor to **Stretch**
4. Set all offsets to 0
5. **IMPORTANT**: In the Image component:
   - Set **Image Type** to **Filled**
   - Set **Fill Method** to **Radial 360**
   - Set **Fill Origin** to **Top**
   - Check **Clockwise**
   - Set Fill Amount to 1

### Step 5: Add Optional Percentage Text
1. Right-click on StaminaMeter → **UI** → **Text** (name it "StaminaText")
2. Center it in the radial meter
3. Set Font Size to 24-32
4. Set Alignment to Center/Middle
5. Set color to white
6. Set text to "100%" (will be updated by script)

### Step 6: Attach the Script
1. Select the **StaminaMeter** GameObject
2. In Inspector, click **Add Component**
3. Search for "StaminaUI" and add it
4. Assign references:
   - **Player Controller**: Drag your character with ThirdPersonController
   - **Radial Fill Image**: Drag the "RadialFill" GameObject
   - **Background Image**: Drag the "Background" GameObject (optional)
   - **Stamina Text**: Drag the "StaminaText" GameObject (optional)

### Step 7: Customize Colors (Optional)
In the StaminaUI component, you can customize:
- **Full Color**: Green (default) - when stamina is high
- **Medium Color**: Yellow (default) - when stamina is medium
- **Low Color**: Red (default) - when stamina is low
- **Low Threshold**: 0.25 (25% stamina)
- **Medium Threshold**: 0.5 (50% stamina)

### Step 8: Test It!
1. Press Play
2. Jump and glide to see stamina drain
3. Land to see it regenerate
4. Watch the color change and pulse effect when low!

---

## 🎨 Visual Customization Tips

### Make it Look Premium:
1. **Add a border**: Create another Image as background with slightly larger size
2. **Use sprites**: Import circular sprite images for better visuals
3. **Add glow**: Duplicate RadialFill, make it slightly larger, reduce alpha
4. **Add icons**: Place a stamina icon in the center

### Recommended Sprites:
- Search for "circle sprite" or "radial gauge sprite" in Unity Asset Store
- Or create your own in Photoshop/GIMP (512x512 circle PNG)

### Color Schemes:
**Sci-Fi Blue:**
- Full: RGB(0, 200, 255)
- Medium: RGB(100, 150, 255)
- Low: RGB(255, 100, 200)

**Nature Green:**
- Full: RGB(50, 255, 100)
- Medium: RGB(255, 200, 50)
- Low: RGB(255, 80, 50)

**Dark Mode:**
- Background: RGB(20, 20, 25) with 80% alpha
- Full: RGB(100, 255, 150)
- Medium: RGB(255, 180, 50)
- Low: RGB(255, 60, 60)

---

## 🔧 Troubleshooting

**Problem**: Radial meter doesn't fill correctly
- **Solution**: Make sure Image Type is set to "Filled" and Fill Method is "Radial 360"

**Problem**: Colors don't change
- **Solution**: Check that Full/Medium/Low colors are different in the inspector

**Problem**: No stamina updates
- **Solution**: Make sure Player Controller is assigned in the StaminaUI component

**Problem**: UI too small/large
- **Solution**: Adjust the Width/Height of StaminaMeter GameObject (try 100-200)

**Problem**: UI not visible
- **Solution**: Check Canvas is set to "Screen Space - Overlay" and UI is in front of camera

---

## 🎮 Features Included

✅ **Radial/Circular Meter** - Smooth 360° fill animation
✅ **Color Coding** - Green → Yellow → Red based on stamina level
✅ **Smooth Transitions** - Animated fill amount changes
✅ **Pulse Effect** - Pulses when stamina is low (warning!)
✅ **Regen Glow** - Glows brighter when regenerating
✅ **Percentage Text** - Optional text display (0-100%)
✅ **Fully Customizable** - All colors, thresholds, and effects adjustable

---

## 📱 Mobile/Console Positioning

**Bottom-Left Corner:**
- Anchor: Bottom-Left
- Pos X: 100, Y: 100

**Bottom-Right Corner:**
- Anchor: Bottom-Right
- Pos X: -100, Y: 100

**Top-Right Corner:**
- Anchor: Top-Right
- Pos X: -100, Y: -100

**Center-Bottom:**
- Anchor: Bottom-Center
- Pos X: 0, Y: 100

Enjoy your beautiful stamina meter! 🎉
