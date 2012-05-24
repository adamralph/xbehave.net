#  Copyright (c) Adam Ralph. All rights reserved.

param($installPath, $toolsPath, $package, $project)

# remove content hook from project and delete file
$hookName = "StyleCop.MSBuild.ContentHook.txt"
$project.ProjectItems.Item($hookName).Remove();
Split-Path $project.FullName -parent | Join-Path -ChildPath $hookName | Remove-Item

# save any unsaved changes to project before we start messing about with project file
$project.Save()

# read in project XML
$projectXml = [xml](Get-Content $project.FullName)
$namespace = 'http://schemas.microsoft.com/developer/msbuild/2003'

# remove current import nodes
$nodes = @(Select-Xml "//msb:Project/msb:Import[contains(@Project,'\packages\StyleCop.MSBuild.')]" $projectXml -Namespace @{msb = $namespace} | Foreach {$_.Node})
if ($nodes)
{
    foreach ($node in $nodes)
    {
        $node.ParentNode.RemoveChild($node)
    }
}

# work out relative path to targets
$absolutePath = Join-Path $toolsPath "StyleCop.targets"
$absoluteUri = New-Object -typename System.Uri -argumentlist $absolutePath
$projectUri = New-Object -typename System.Uri -argumentlist $project.FullName
$relativeUri = $projectUri.MakeRelativeUri($absoluteUri)
$relativePath = [System.URI]::UnescapeDataString($relativeUri.ToString()).Replace('/', '\');

# add new import
$import = $projectXml.CreateElement('Import', $namespace)
$import.SetAttribute('Project', $relativePath)
$projectXml.Project.AppendChild($import)

# save changes
$projectXml.Save($project.FullName)