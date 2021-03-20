$versionXml=([xml](Get-Content .\version.props))
$versionProperty=$versionXml.Project.PropertyGroup
$version=$versionProperty.VersionMajor+"."+$versionProperty.VersionMinor+"."+$versionProperty.VersionPatch
$env:ReleaseVersion=$version
$env:ReleaseVersion