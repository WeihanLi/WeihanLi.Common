var currentDirectory = Directory.GetCurrentDirectory();
Console.WriteLine($"currentDirectory: {currentDirectory}");

var versionPropsFile = "./build/version.props";
var doc = System.Xml.Linq.XDocument.Load(versionPropsFile);
var propertyGroupNode = doc.Element("Project").Element("PropertyGroup");

var version = $"{propertyGroupNode.Element("VersionMajor").Value}.{propertyGroupNode.Element("VersionMinor").Value}.{propertyGroupNode.Element("VersionPatch").Value}";
Console.WriteLine($"Version: {version}");

var envFile = Environment.GetEnvironmentVariable("GITHUB_ENV");
Console.WriteLine($"EnvFilePath: {envFile}");

if (string.IsNullOrEmpty(envFile)) return;

File.WriteAllText(envFile, $"ReleaseVersion={version}");
Console.WriteLine(File.ReadAllText(envFile));
