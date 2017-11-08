# xamarin-native-tile-template
Native tile template for Xamarin platform

Warning: this is very early version and serious changes can be applied to template.

## Structure:

```
Solution
    |-PlatformTile // Contains tile logic and can be modified to have custom workflow or to fix bug
        |-Moduware.Platform.Tile.Droid // Android specific code
        |-Moduware.Platform.Tile.iOS // iOS specific code
        |-Moduware.Platform.Tile.Share // main, shared platform independent code
    |-XamarinAndroidTileTemplate // Starting point for your Android tile, by default contains code for LED
    |-XamariniOSTileTemplate // Starting point for you iOS tile, by default contains code for LED
```

Also there is folder `moduware.tile.template`. To make Moduware app recogrnise your native tile, it should be accompanied
by meta data (icon, manifest.json). It can go from repository or from developer's folder on device. To make Moduware app recognise
the template, copy `moduware.tile.template` folder to developer's folder (you will need restart app after it).
