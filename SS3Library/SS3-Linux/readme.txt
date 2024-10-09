Fri Jun 30 12:35:02 PM CDT 2023
# SecuGen SecuSearch 3 SDK for Linux
## Version 3.2.7 Beta 5
### README
================================================================
			
### Supported Platforms
======================
Linux linuxmint 5.15.0-73-generic #80-Ubuntu SMP Mon May 15 15:18:26 UTC 2023 x86_64 x86_64 x86_64 GNU/Linux
++ (Ubuntu 11.3.0-1ubuntu1~22.04.1) 11.3.0

### Files Included in this Distribution
=======================================
readme.txt          		- this file
bin/linux4X64
	run.sh		            - Script to run sseapitest
	sseapitest	            - SecuSearch C++ 64bit sample application
	volume_number_generator	- Volume number reader for licensing
include/linux4X64
	SecuSearchAPI.h		    - SecuSearch SDK header file
	SecuSearchAPI.hpp	    - SecuSearch SDK header file
	SecuSearchDef.h		    - SecuSearch SDK header file
	SecuSearchErr.h		    - SecuSearch SDK header file
    SecuSearchException.hpp - SecuSearch SDK header file
min_data/
	Directory containing fingerprint template files used by sample application
sseapitest_cpp
	C++ source code for sseapitest
test_data\
	ANSI and ISO template test data 
SecuSearch 3 SDK Pro Manual (Windows) SG1-0005D-006.pdf - SecuSearch SDK documentation

### Installation
================
1. Unzip the SecuSearch distribution
2. cd lib/linux4X64
   make uninstall install
3. Obtain a license file if you think that the application requires greater than 1,000 templates to be stored in DB.  Proceed as follows:
	Step 1)	Run 'volume_number_generator' in the bin directory of this distribution.
	Step 2) Save the volume number of your system as displayed by this application.
	Step 3) Send an email including the volume number to your reseller or SecuGen Corporation to request a license file.
	Step 4) Copy the license file (license.dat) into the bin directory of this distribution or the directory 
            where your application is running.
	Step 5) Specify the license file name in ENGINE_PARAM structure on calling InitializeEngine.

### Release Notes
=================
v3.2.7 Beta5 - Linux release.
v3.2.4 - Allowance of DB to have up to 1,000 templates w/o license file.  After 1,000 templates in DB, license file drives max templates.
v3.2.3 - Fixed problem loading templates with small number of minutiae
v3.2.2 - Fixed problem with SeacuSearch.NET running on IIS 
v3.2 - Added support for 32bit Windows
       Maximum number of fingerprints supported on 32bit platforms is 100,000.
v3.1 - .Net API support
v3.0 - Initial release

	
