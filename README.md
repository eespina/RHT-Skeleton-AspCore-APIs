# RHT-Skeleton-Angular-AspCore-APIs
Owners Portal for eVillage Saas product. TEMPORARILY on hold while creation of the MVC .NET Core version (with Views) is being created. Eventually material will transfer to this version.

# Setting Up

	- Pull down the repo
	- Travel to the directory where the 'package.json' file is located
	- Open a shell and run "npm install"
	- In the shell, 'cd' to wherever the 'gulpfile.,js' is located (if not there in the ssame directory already) and run 'gulp' to begin the Gulp task-runner
	- If there is an error concerning an 'UnhandledPromiseRejectionWarning', try typing 'gulp' again to retry the gulp process
	
	- Open Visual Studio and build the solution
	- In Visual Studio, set the AngularFroenEnd's Start page as 'index.html'
	- Note that a token must be presented for accessing areas that require authorization/permission
	
	- Ensure your database connections are correct and that they are Running
	
	- For helpful debugging and to run both projects at the same time, go to the Visual Studio properties on the Solution level and choose "Multiple startup projects"
	