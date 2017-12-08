# Native Tile Template (Xamarin)
Native tile template for Moduware platform using Xamarin technology.
This template uses, two nuget packages plus some logic sugar, you can use it as starting point for your project or just as reference.
- [Moduware.Platform.Core](https://www.nuget.org/packages/Moduware.Platform.Core/) - general platform library
- [Moduware.Platform.Tile](https://www.nuget.org/packages/Moduware.Platform.Tile/) - basic tile behaviour framework

## Structure:

- Droid - Android specific project and code
- iOS - iOS specific project and code
- Shared - Shared logic, code and data
- moduware.tile.template - tile meta data for developer's mode or repository

To make Moduware app recogrnise your native tile, it should be accompanied by meta data (icon, manifest.json) in folder named same as Id of your tile. It can go from repository or from developer's folder on device. To make Moduware app recognise
the template, copy `moduware.tile.template` folder to developer's folder (you will need restart app after it).
