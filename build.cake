///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var stable = Argument("stable", "false");

var branchName = EnvironmentVariable("BUILD_SOURCEBRANCHNAME") ?? "local";
var isWindowsAgent = EnvironmentVariable("Agent_OS") == "Windows_NT" || branchName == "local";

var solutionPath = "./WeihanLi.Common.sln";
var srcProjects  = GetFiles("./src/**/*.csproj");
var testProjects = GetFiles("./test/**/*.csproj");
var packProjects = GetFiles("./src/**/*.csproj");

var artifacts = "./artifacts/packages";

///////////////////////////////////////////////////////////////////////////////
// SETUP / TEARDOWN
///////////////////////////////////////////////////////////////////////////////

Setup(ctx =>
{
   // Executed BEFORE the first task.
   Information("Running tasks...");
   PrintBuildInfo(ctx);
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


Task("test")    
    .Description("Tests")
    .IsDependentOn("build")
    .Does(() =>
    {
      var testSettings = new DotNetCoreTestSettings
      {
        NoRestore = false,
        Configuration = configuration
      };
      foreach(var project in testProjects)
      {
        DotNetCoreTest(project.FullPath, testSettings);
      }
    });

Task("pack")
    .Description("Pack package")
    .IsDependentOn("test")
    .Does((cakeContext) =>
    {
      var settings = new DotNetCorePackSettings
      {
         Configuration = configuration,
         OutputDirectory = artifacts,
         VersionSuffix = "",
         NoRestore = true,
         NoBuild = true
      };
      if(branchName != "master" && stable != "true"){
         settings.VersionSuffix = $"preview-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
      }
      foreach (var project in packProjects)
      {
         DotNetCorePack(project.FullPath, settings);
      }
      PublishArtifacts(cakeContext);
    });

bool PublishArtifacts(ICakeContext context)
{
   if(context.Environment.Platform.IsUnix())
   {
      Information($@"none windows build agent, do not publish packages");
      return false;
   }
   if(branchName == "master" || branchName == "preview")
   {
      var pushSetting =new DotNetCoreNuGetPushSettings
      {
         SkipDuplicate = true,
         Source = EnvironmentVariable("Nuget__SourceUrl") ?? "https://api.nuget.org/v3/index.json",
         ApiKey = EnvironmentVariable("Nuget__ApiKey")
      };
      var packages = GetFiles($"{artifacts}/*.nupkg");
      foreach(var package in packages)
      {
         DotNetCoreNuGetPush(package.FullPath, pushSetting);
      }
      return true;
   }
   Information($@"branch name does not match, do not publish packages");
   return false;
}

void PrintBuildInfo(ICakeContext context){
   Information($@"branch:{branchName}, agentOs={EnvironmentVariable("Agent_OS")},Platform: {context.Environment.Platform.Family}, IsUnix: {context.Environment.Platform.IsUnix()}
   BuildID:{EnvironmentVariable("BUILD_BUILDID")},BuildNumber:{EnvironmentVariable("BUILD_BUILDNUMBER")},BuildReason:{EnvironmentVariable("BUILD_REASON")}
   ");
}

Task("Default")
    .IsDependentOn("pack");

RunTarget(target);
