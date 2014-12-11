## Setup

* Rename "dobuild_env.template" into "dobuild_env.bat"
* Set the VS_VERSION variable to the correct Visual Studio version
    * Visual Studio 2010: 10.0
    * Visual Studio 2012: 11.0
    * Visual Studio 2013: 12.0
    
## Usage 

The various utilities are:

* dobuild.bat: Build all projects and generate all the nuget packages under "tmp_nuget" directory
* zipproject.bat: Zip the project into ..\..\[ProjectName].[YYYYmmdd].[hhss].zip without bin, obj, nuget packages, test results
* zipproject.nuget.bat: As zipproject.bat but including the nuget packages directory
* setcopyright.bat: Set the copyright on the solution .cs files. The copyright notice is in the license.cs file

## Debugging

While debugging, all files will be kept! No cleanup will be made!

To debug the whole build process the variable VERBOSITY can be set to TRUE (mind the capital letters!) in dobuild_env.bat

To debug single parts of the build the single line in dobuild.bat can be surrounded like this

    ...
    SET VERBOSITY=TRUE
    call dobuild_single ConcurrencyHelpers 4.0 net40 src\ConcurrencyHelpers
    SET VERBOSITY=FALSE
    ...
    
At this point only the build of ConcurrencyHelpers with framework 4.0 will be shown as verbose
    
## Extra files

* license.cs: The copyright notice to prepend on all .cs files
* dobuild_env.bat: The Visual Studio environment variables
* dobuild_*.bat: Utilities to build the project (not intented for direct usage)
* VisualStudioIdentifier.exe: Utility to find the location of devenv.exe
* BuildCleaner.exe: Utility to find the solution files and zip them
* CommentsHeader.exe: Utility to find add comments on all files