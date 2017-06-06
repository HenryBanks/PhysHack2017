# PhysHack2017 - MindGames

Our project looked at controlling a computer using keypresses
triggered by EEG headsets, in particular the EMOTIV EPOC+ and NeuroSky
Mindwave Mobile.

## Code
The repository has two main components. The bulk of it is the Unity
Project `MindGames`; this contains the source code, and a Mac and
Windows Native executables, for a simple flappy bird-esque paper plane
game.

The additional file, `Algo Sdk Sample.cpp`, is to be used in
conjunction with the NeuroSky Algo SDK Example project, available as
part of
the
[Windows developer tools](http://developer.neurosky.com/docs/doku.php?id=mdt2.5). To compile, replace the executable in the Algo SDK Visual Studio project with the version from this repository, and build and run the solution. Select the 'Blink' functionality and connect the headset.

All EMOTIV control is handled by the `Emokey` software.
