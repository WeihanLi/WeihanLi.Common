$versionPath=$PSScriptRoot+"/version.props"
$versionXml=([xml](Get-Content $versionPath))
$versionProperty=$versionXml.Project.PropertyGroup
$ReleaseVersion=$versionProperty.VersionMajor+"."+$versionProperty.VersionMinor+"."+$versionProperty.VersionPatch
$ReleaseVersion
Add-Content -Path $env:GITHUB_ENV -Value "ReleaseVersion=${ReleaseVersion}"