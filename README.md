# Weevil

Weevil is a Grasshopper plugin to assist and simplify 2D fabrication workflow, it provides tools for curve processing & cleanup, nesting, toolpath generation, and G-code export.
Currently in active development, with the first set of components centered on curve cleanup and preparation.

## in development 

Curve Processing & Cleanup

## Planned Features 

2D nesting (sheet optimization)
Toolpath generation
G-code export

## Why Weevil?

In 2D fabrication (CNC, laser, plasma, etc.), cleaning curves is essential becasue imported geometry is often messy:
- Exploded into thousands of tiny segments
- Slightly misaligned due to precision or export issues
- Full of overlaps, duplicates, and small gaps

This causes problems downstream and result in wasted time in manual cleanup 

Weevil fixes this directly in Grasshopper with a focused, fabrication-ready toolset:
- Robust curve cleanup and continuity restoration
- Efficient nesting (coming soon)
- Smart toolpath generation (coming soon)
- Clean G-code export (coming soon)

The goal: turn unreliable imports into reliable, machine-ready geometry.

Curve processing is the current focus, with nesting, toolpaths, and G-code next.