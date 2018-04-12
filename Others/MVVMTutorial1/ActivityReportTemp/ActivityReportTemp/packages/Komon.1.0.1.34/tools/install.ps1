param($installPath, $toolsPath, $package)
$SolutionDir = $installPath.Substring(0,$installPath.IndexOf("packages\"))
$templatesPath = $SolutionDir + "CodeTemplates\"
$komonPath = $SolutionDir + "CodeTemplates\Komon"
$msbuildPath = $SolutionDir + "CodeTemplates\T4MSBuild"
$tsPath = $SolutionDir + "TypeScript\"

$templatesPathExists = Test-Path -Path $templatesPath
if($templatesPathExists -eq $false)
{
    mkdir -Path $templatesPath
}
if((Test-Path -Path $komonPath) -eq $false)
{
    mkdir -Path $komonPath
}
if((Test-Path -Path $msbuildPath) -eq $false)
{
    mkdir -Path $msbuildPath
}
if((Test-Path -Path $tsPath) -eq $false)
{
    mkdir -Path $tsPath
}
$fromPath =  $toolsPath + "\templates\" 

copy ($fromPath + "TemplateClientInProcess.tt") $templatesPath
copy ($fromPath + "TemplateClientInterface.tt") $templatesPath
copy ($fromPath + "TemplateClientRest.tt") $templatesPath
copy ($fromPath + "TemplateClientTypeScript.tt") $templatesPath
copy ($fromPath + "TemplateEfInterfaces.tt") $templatesPath
copy ($fromPath + "TemplateEfPocos.tt") $templatesPath
copy ($fromPath + "TemplateModelResources.tt") $templatesPath
copy ($fromPath + "TemplateServiceWebApi.tt") $templatesPath

copy ($fromPath + "ts\KomonFramework.ts") $tsPath
copy ($fromPath + "ts\KomonLoader.ts") $tsPath
copy ($fromPath + "ts\Komon.Clients.ts") $tsPath

copy ($fromPath + "komon\Komon.TextTemplates.dll") $komonPath
copy ($fromPath + "komon\Microsoft.CodeAnalysis.CSharp.Desktop.dll") $komonPath
copy ($fromPath + "komon\Microsoft.CodeAnalysis.CSharp.dll") $komonPath
copy ($fromPath + "komon\Microsoft.CodeAnalysis.Desktop.dll") $komonPath
copy ($fromPath + "komon\Microsoft.CodeAnalysis.dll") $komonPath
copy ($fromPath + "komon\System.Collections.Immutable.dll") $komonPath
copy ($fromPath + "komon\System.Reflection.Metadata.dll") $komonPath

copy ($fromPath + "t4msbuild\Microsoft.TextTemplating.Build.Tasks.dll") $msbuildPath
copy ($fromPath + "t4msbuild\Microsoft.TextTemplating.targets") $msbuildPath
copy ($fromPath + "t4msbuild\Microsoft.VisualStudio.TextTemplating.12.0.dll") $msbuildPath
copy ($fromPath + "t4msbuild\Microsoft.VisualStudio.TextTemplating.Interfaces.10.0.dll") $msbuildPath
copy ($fromPath + "t4msbuild\Microsoft.VisualStudio.TextTemplating.Interfaces.11.0.dll") $msbuildPath
copy ($fromPath + "t4msbuild\Microsoft.VisualStudio.TextTemplating.Interfaces.12.0.dll") $msbuildPath
copy ($fromPath + "t4msbuild\Microsoft.VisualStudio.TextTemplating.Modeling.12.0.dll") $msbuildPath
copy ($fromPath + "t4msbuild\Microsoft.VisualStudio.TextTemplating.Sdk.Host.12.0.dll") $msbuildPath
copy ($fromPath + "t4msbuild\Microsoft.VisualStudio.TextTemplating.VSHost.12.0.dll") $msbuildPath



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