var versionPropsFile = "./version.props";
var doc = System.Xml.Linq.XDocument.Load(path);
var propertyGroupNode = doc.Element("Project").Element("PropertyGroup");

var version = $"{propertyGroupNode.Element("VersionMajor").Value}.{propertyGroupNode.Element("VersionMinor").Value}.{propertyGroupNode.Element("VersionPatch").Value}";
Console.WriteLine($"Release version: {version}");

var envFile = Environment.GetEnvironmentVariable("GITHUB_ENV");
Console.WriteLine($"EnvFilePath: {envFile}");

File.WriteAllText(envFile, $"ReleaseVersion={version}");
Console.WriteLine(File.ReadAllText(envFile));
