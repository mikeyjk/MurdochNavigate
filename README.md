MurdochNavigate
===============

Interactive, intelligent navigational assistance for Murdoch University Campus (South Street).

To Do:

1) Drop a marker at certain locations (track parked car).
2) Fix camera's magnetic heading.
3) Add screenshots / video demonstration.
4) Create a binary directory for Android .apk and iOS exectuable etc.

To open and run/compile this project in Unity:

    git clone https://github.com/mikeyjk/MurdochNavigate

Edit Assets/Src/GoogleMaps.cs on Line 48 to include a Google Static Maps API Key:

    static readonly private string apiKey = "key_goes_here";

Receiving a Google Static Maps API key is quite simple, as well as free. Please consult Google documentation if you wish to do this. I was tempted to include my own because there is little chance of someone abusing my key, but it isn't really recommended in the development community.

If you wish to merely use the application, I going to add the 'binary' (.apk) and upload to the Google Play Store when I have the free time.
