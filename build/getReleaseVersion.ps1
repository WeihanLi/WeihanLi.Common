$versionPath=$PSScriptRoot+"/version.props"
$versionXml=([xml](Get-Content $versionPath))
$versionProperty=$versionXml.Project.PropertyGroup
$ReleaseVersion=$versionProperty.VersionMajor+"."+$versionProperty.VersionMinor+"."+$versionProperty.VersionPatch
$ReleaseVersion
echo "ReleaseVersion=${ReleaseVersion}" >> $GITHUB_ENV