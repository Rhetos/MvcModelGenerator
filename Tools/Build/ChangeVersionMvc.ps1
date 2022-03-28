param([string]$version, [string]$prerelease)

If ($prerelease -eq 'auto')
{
    $prerelease = ('dev'+(get-date -format 'yyMMddHHmm')+(git rev-parse --short HEAD)).Substring(0,20)
}

If ($prerelease)
{
    $fullVersion = $version + '-' + $prerelease
}
Else
{
    $fullVersion = $version
}

function RegexReplace ($fileSearch, $replacePattern, $replaceWith)
{
    Get-ChildItem $fileSearch -r `
        | ForEach-Object {
        $c = [IO.File]::ReadAllText($_.FullName) -Replace $replacePattern, $replaceWith;
        [IO.File]::WriteAllText($_.FullName, $c)
    }
}

Write-Output "Setting version '$fullVersion'."

RegexReplace '*.nuspec' '([\n^]\s*\<version\>).*(\<\/version\>\s*)' ('${1}'+$fullVersion+'${2}')
RegexReplace 'Rhetos.MvcModelGenerator.nuspec' '(Rhetos.MvcModelGenerator.Client\" version=\").*(\")' ('${1}'+$fullVersion+'${2}')
RegexReplace 'Directory.Build.props' '([\n^]\s*\<InformationalVersion\>).*(\<\/InformationalVersion\>\s*)' ('${1}' + $fullVersion + '${2}')
RegexReplace 'Directory.Build.props' '([\n^]\s*\<AssemblyVersion\>).*(\<\/AssemblyVersion\>\s*)' ('${1}' + $version + '${2}')
RegexReplace 'Directory.Build.props' '([\n^]\s*\<FileVersion\>).*(\<\/FileVersion\>\s*)' ('${1}' + $version + '${2}')
