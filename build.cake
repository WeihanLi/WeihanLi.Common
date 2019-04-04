///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var solutionPath = "./WeihanLi.Common.sln";
var srcProjects  = GetFiles("./src/**/*.csproj");

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
   // Executed BEFORE the first task.
   Information("Running tasks...");
});

Teardown(ctx =>
{
   // Executed AFTER the last task.
   Information("Finished running tasks.");
});

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////


Task("clean")
    .Description("Clean")
    .Does(() =>
    {
      if (DirectoryExists("./artifacts"))
      {
         DeleteDirectory("./artifacts", true);
      }
    });

Task("restore")
    .Description("Restore")
    .Does(() => 
    {
       DotNetCoreRestore(solutionPath);
    });

Task("build")    
    .Description("Build")
    .IsDependentOn("clean")
    .IsDependentOn("restore")
    .Does(() =>
    {
      DotNetCoreRestore(solutionPath);
    });

Task("pack")
    .Description("Pack package")
    .IsDependentOn("build")
    .Does(() =>
    {
      var settings = new DotNetCorePackSettings
      {
         Configuration = configuration,
         OutputDirectory = "./artifacts/packages"
      };
      foreach (var project in srcProjects)
      {
         DotNetCorePack(project.FullPath, settings);
      }
    });

Task("Default")
    .IsDependentOn("build");

RunTarget(target);