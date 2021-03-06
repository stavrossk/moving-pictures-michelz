# This script is designed to create a release version of Moving Pictures.

# To run this script you need the latest version of NSIS installed as well as the 
# the XML plug-in for NSIS.

# This script also assumes that movingpictures.dll has been built and merged. 
# If this is not true, the script will fail or the DLL installed will not run
# properly! Automatic building of the movingpictures.dll will be implemented 
# in an upcoming version of this script.

!define RELEASE
!include Setup.nsh