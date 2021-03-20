$versionPath=$PSScriptRoot+"/version.props"
$versionXml=([xml](Get-Content $versionPath))
$versionProperty=$versionXml.Project.PropertyGroup
$env:ReleaseVersion=$versionProperty.VersionMajor+"."+$versionProperty.VersionMinor+"."+$versionProperty.VersionPatch
$env:ReleaseVersion