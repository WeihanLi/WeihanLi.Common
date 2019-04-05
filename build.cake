///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var solutionPath = "./WeihanLi.Common.sln";
var srcProjects  = GetFiles("./src/**/*.csproj");

var artifacts = "./artifacts/packages";
var isPr = int.TryParse(EnvironmentVariable("System.PullRequest.PullRequestNumber"), out var prId) && prId > 0;
var isWindowsAgent = (EnvironmentVariable("Agent_OS") ?? "Windows_NT") == "Windows_NT";
var branchName = EnvironmentVariable("BUILD_SOURCEBRANCHNAME") ?? "dev";

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
       var deleteSetting = new DeleteDirectorySettings()
       {
          Force = true,
          Recursive = true
       };
      if (DirectoryExists(artifacts))
      {
         DeleteDirectory(artifacts, deleteSetting);
      }
    });

Task("restore")
    .Description("Restore")
    .Does(() => 
    {
      foreach(var project in srcProjects)
      {
         DotNetCoreRestore(project.FullPath);
      }
    });

Task("build")    
    .Description("Build")
    .IsDependentOn("clean")
    .IsDependentOn("restore")
    .Does(() =>
    {
      var buildSetting = new DotNetCoreBuildSettings{
         NoRestore = true,
         Configuration = configuration
      };
      foreach(var project in srcProjects)
      {
         DotNetCoreBuild(project.FullPath, buildSetting);
      }
    });

Task("pack")
    .Description("Pack package")
    .IsDependentOn("build")
    .Does(() =>
    {
      var settings = new DotNetCorePackSettings
      {
         Configuration = configuration,
         OutputDirectory = artifacts,
         VersionSuffix = "",
         NoRestore = true,
         NoBuild = true
      };
      if(branchName != "master"){
         settings.VersionSuffix = $"preview-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
      }
      Information($"branch:{branchName}, isPr:{isPr}, isWindows={isWindowsAgent}");
      foreach (var project in srcProjects)
      {
         DotNetCorePack(project.FullPath, settings);
      }
      PublishArtifacts();
    });

bool PublishArtifacts(){
   if(isPr || !isWindowsAgent){
      return false;
   }
   if(branchName == "master" || branchName == "preview"){
      var pushSetting =new DotNetCoreNuGetPushSettings
      {
         Source = EnvironmentVariable("nugetSourceUrl") ?? "https://api.nuget.org/v3/index.json",
         ApiKey = EnvironmentVariable("nugetApiKey")
      };
      DotNetCoreNuGetPush($"{artifacts}/*.nupkg", pushSetting);
      return true;
   }
   return false;
}

Task("Default")
    .IsDependentOn("pack");

RunTarget(target);