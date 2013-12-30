param($installPath, $toolsPath, $package, $project)
# Set the build action type to None
$project.ProjectItems.Item("T4DocoptNet.tt").Properties.Item("BuildAction").Value = 0 #prjBuildActionNone
$project.ProjectItems.Item("T4DocoptNet.tt.hooks.t4").Properties.Item("BuildAction").Value = 0
$project.ProjectItems.Item("T4DocoptNet.tt.settings.xml").Properties.Item("BuildAction").Value = 0
