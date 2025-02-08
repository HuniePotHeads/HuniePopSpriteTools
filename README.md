# HuniePop Sprite Tools

Several tools for dealing with HuniePop's "Toolkit2D" sprite collections.

Mostly hacked together somewhere in 2015 and last updated in 2019.

## HOW TO USE

### For Backgrounds and Photos

- Dump sprite collection from game (see format in [Collection2Sprites/SpriteInfo.cs](Collection2Sprites/SpriteInfo.cs#L53))
- Run  1-Collection2Sprites.exe
- Run  2-Sprites2Stitch.exe
- Modify stitched image
- Run  3-Stitch2Sprites.exe
- Run  4-Sprites2Collection.exe

### For Girls

- Dump sprite collection from game
- Run  1-Collection2Sprites.exe
- Modify any sprite
- Run  4-Sprites2Collection.exe

## NOTES
- Do NOT move or rename any file.
- Do NOT change the dimensions (width, height) of any image.
- All programs work with *.png files. "Stitch2Sprites" will additionally look for a stitched *.jpg file.
- The coordinates in the CSV files are for flipped textures, this is what the DDS textures in the game's resources are.
  HunieMod dumps the texture "Unflipped" (or, upright) because that's how the textures are rendered.
  "Collection2Sprites" looks for a "*_Unflipped.png" file when the normal one isn't found.
  However, it supports both:
    - PhotosKyuSpriteCollection.png           : Default orientation, Collection2Sprites.exe will use it as is.
    - PhotosKyuSpriteCollection_Unflipped.png : Upright texture, Collection2Sprites.exe will flip it vertically before using it.
