///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
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
         DotNetRestore(project.FullPath);
      }
    });

Task("build")    
    .Description("Build")
    .IsDependentOn("clean")
    .IsDependentOn("restore")
    .Does(() =>
    {
      var buildSetting = new DotNetBuildSettings{
         NoRestore = true,
         Configuration = configuration
      };
      foreach(var project in srcProjects)
      {
         DotNetBuild(project.FullPath, buildSetting);
      }
    });


Task("test")    
    .Description("Tests")
    .IsDependentOn("build")
    .Does(() =>
    {
      var testSettings = new DotNetTestSettings
      {
        NoRestore = false,
        Configuration = configuration
      };
      foreach(var project in testProjects)
      {
        DotNetTest(project.FullPath, testSettings);
      }
    });

Task("pack")
    .Description("Pack package")
    .IsDependentOn("test")
    .Does((cakeContext) =>
    {
      var settings = new DotNetPackSettings
      {
         Configuration = "Release",
         OutputDirectory = artifacts,
         VersionSuffix = "",
         NoRestore = true,
      };
      if(branchName != "master" && stable != "true"){
         settings.VersionSuffix = $"preview-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
      }
      foreach (var project in packProjects)
      {
         DotNetPack(project.FullPath, settings);
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
      var pushSetting =new DotNetNuGetPushSettings
      {
         SkipDuplicate = true,
         Source = EnvironmentVariable("Nuget__SourceUrl") ?? "https://api.nuget.org/v3/index.json",
         ApiKey = EnvironmentVariable("Nuget__ApiKey")
      };
      var packages = GetFiles($"{artifacts}/*.nupkg");
      foreach(var package in packages)
      {
         DotNetNuGetPush(package.FullPath, pushSetting);
      }
      return true;
   }
   Information($@"branch name does not match, do not publish packages");
   return false;
}

void PrintBuildInfo(ICakeContext context){
   Information($@"branch:{branchName},Platform: {context.Environment.Platform.Family}, IsUnix: {context.Environment.Platform.IsUnix()}
   BuildID:{EnvironmentVariable("BUILD_BUILDID")},BuildNumber:{EnvironmentVariable("BUILD_BUILDNUMBER")},BuildReason:{EnvironmentVariable("BUILD_REASON")}
   ");
}

Task("Default")
    .IsDependentOn("pack");

RunTarget(target);
