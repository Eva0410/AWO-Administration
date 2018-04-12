param($installPath, $toolsPath, $package)
Import-Module (Join-Path $toolsPath updatekomon.psm1)

$fromPath =  $toolsPath + "\templates\" 
$snippCSharp13 =[environment]::getfolderpath("mydocuments") + "\Visual Studio 2013\Code Snippets\Visual C#\My Code Snippets\"
if((Test-Path -Path $snippCSharp13) -eq $true)
{
    copy ($fromPath + "snippets\kPropPC.snippet") $snippCSharp13
}
$snippCSharp15 =[environment]::getfolderpath("mydocuments") + "\Visual Studio 2015\Code Snippets\Visual C#\My Code Snippets\"
if((Test-Path -Path $snippCSharp15) -eq $true)
{
    copy ($fromPath + "snippets\kPropPC.snippet") $snippCSharp15
}