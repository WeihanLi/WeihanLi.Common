$versionPath=$PSScriptRoot+"/version.props"
$versionXml=([xml](Get-Content $versionPath))
$versionProperty=$versionXml.Project.PropertyGroup
$ReleaseVersion=$versionProperty.VersionMajor+"."+$versionProperty.VersionMinor+"."+$versionProperty.VersionPatch
$ReleaseVersion
echo "ReleaseVersion=${ReleaseVersion}" >> $Env.GITHUB_ENV
echo "ReleaseVersion1=${ReleaseVersion}" >> $Env.$GITHUB_ENV
echo "ReleaseVersion2=${ReleaseVersion}" >> $GITHUB_ENV