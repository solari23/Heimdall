##Heimdall

Heimdall is a secure central access point for home automation APIs.




######Universal Windows App Clients Setup

Several projects in this repository are Universal Windows Apps which require a key to
publish the package to the local device for debugging. To uphold good security practice,
the PFX files containing the keys are not checked in to version control.

To regenerate the certificates:

1. Open the Visual Studio solution containing the project
2. From Solution Explorer, open the "Package.appxmanifest" file in the app's project
3. In the main window, switch to the "Packaging" tab
4. Next to "Publisher", choose "Choose Certificate"
6. Change the dropdown to "Create Test Certificate"
7. Pick a publisher name (or just keep the defaults) and hit "OK"

