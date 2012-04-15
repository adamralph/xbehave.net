param($installPath, $toolsPath, $package, $project)

# save any unsaved changes before we start messing about with the project file on disk
$project.Save()

# read in project XML
$projectXml = [xml](Get-Content $project.FullName)
$namespace = 'http://schemas.microsoft.com/developer/msbuild/2003'

# remove import nodes
$nodes = @(Select-Xml "//msb:Project/msb:Import[contains(@Project,'\packages\StyleCop.MSBuild.')]" $projectXml -Namespace @{msb = $namespace} | Foreach {$_.Node})
if ($nodes)
{
    foreach ($node in $nodes)
    {
        $node.ParentNode.RemoveChild($node)
    }
}

# save changes
$projectXml.Save($project.FullName)