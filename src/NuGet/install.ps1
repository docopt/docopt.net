param($installPath, $toolsPath, $package, $project)
# Set the build action type to None
$project.ProjectItems.Item("T4DocoptNet.tt").Properties.Item("BuildAction").Value = 0 #prjBuildActionNone
$project.ProjectItems.Item("T4DocoptNet.tt.hooks.t4").Properties.Item("BuildAction").Value = 0
$project.ProjectItems.Item("T4DocoptNet.tt.settings.xml").Properties.Item("BuildAction").Value = 0

# Edit the project's T4DocoptNet.tt file to point to the right DocoptNet assembly path.
function Set-DocoptNetAssemblyPath($AssemblyPath, $ProjectDirectoryPath)
{
    # Get the path to the T4DocoptNet.tt file.
    Write-Debug "Project directory is '$ProjectDirectoryPath'."
    $t4FilePath = Join-Path $ProjectDirectoryPath "T4DocoptNet.tt"

    # If we found the T4 file, try and update it.
    if (Test-Path -Path $t4FilePath)
    {
        Write-Debug "Found T4 file at '$t4FilePath'."
    
        # Load the packages.config xml document and grab all of the <package> elements.
        $wholeFile = [System.IO.File]::ReadAllText($t4FilePath)
        $searchString = '\$\(ProjectDir\)\$\(OutDir\)DocoptNet.dll'
        if (!($wholeFile -match $searchString))
        {
            Write-Debug "Could not find search string in '$t4FilePath'."
            return
        }
        
        Write-Debug "Setting assembly path '$AssemblyPath' in '$t4FilePath'."
        $wholeFile = $wholeFile -replace $searchString, $AssemblyPath
  
        # Save the T4 file back now that we've changed it.
        [System.IO.File]::WriteAllText($t4FilePath, $wholeFile)
    }
    # Else we coudn't find the T4 file for some reason, so error out.
    else
    {
        Write-Debug "Could not find T4 file at '$t4FilePath'."
    }
}

$p = '$(SolutionDir)' + "packages\" + $package.Id + "." + $package.Version + "\lib\net40\DocoptNet.dll"
Set-DocoptNetAssemblyPath -AssemblyPath $p -ProjectDirectoryPath ([System.IO.Directory]::GetParent($project.FullName))
