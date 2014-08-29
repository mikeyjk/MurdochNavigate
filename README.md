MurdochNavigate
===============

Interactive, intelligent navigational assistance for Murdoch University Campus (South Street).
Written in C# with the Unity Game Development Engine.
Currently Android is the only tested/supported platform however it was designed with iOS support in mind.

-------------------

**Screenshots:**

[Splash Message Screenshot](https://raw.githubusercontent.com/mikeyjk/MurdochNavigate/master/Documentation/Images/splash.PNG), [Navigate 1](https://raw.githubusercontent.com/mikeyjk/MurdochNavigate/master/Documentation/Images/navigate.PNG), [Navigate 2](https://raw.githubusercontent.com/mikeyjk/MurdochNavigate/master/Documentation/Images/navigate2.PNG), [Navigate 3](https://raw.githubusercontent.com/mikeyjk/MurdochNavigate/master/Documentation/Images/navigate3.PNG)

-------------------

**Installation/Usage:**

To open and run/compile this project in Unity:

    git clone https://github.com/mikeyjk/MurdochNavigate

Edit Assets/Src/GoogleMaps.cs on Line 48 to include a Google Static Maps API Key:

    static readonly private string apiKey = "key_goes_here";

Receiving a Google Static Maps API key is quite simple, as well as free. Please consult Google documentation if you wish to do this. I was tempted to include my own because there is little chance of someone abusing my key, but it isn't really recommended in the development community.

If you wish to merely use the application, I going to add the 'binary' (.apk) and upload to the Google Play Store when I have the free time.

-------------------

**To Do**:

1) Fix camera's magnetic heading.

2) Add video demonstration.

3) Create a binary directory for Android and iOS executable/package.

4) Continue investigation of dynamically generating obstructions on campus. Currently not scaleable.

5) User Interface development.

6) Work on touch screen input.

7) Emulate being on campus from mobile device, somehow.

**Ideas**:

1) Drop a marker at certain locations (track parked car).
 (share locations on facebook? store a local list of markers for user created content)
 
2) Touch a building to view more information about it.

-------------------

**Original Development Team (Q2 2014):**

Michael James Kiernan (Bsc Computer Science, Games Technology)
- Lead Programmer, Lead Designer, Software Architect

Matius Kristiyanto Tjandradjaja (BDM Games Software Design and Production)
- Lead Artist, 3D Modelling, Virtual Environment Design

Jason Byrne (Bsc, Games Software Design and Production)
- Programmer, Developer, Record Keeper

Zach Reynolds (Bsc, Games Software Design and Production)
- Programmer, Designer, Test Driven Development 

Julie Brown (Bsc, Games Software Design and Production)
- Documentation, Development, Project Management
