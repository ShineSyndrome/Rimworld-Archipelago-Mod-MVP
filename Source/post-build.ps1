#Put your local test mod assembly location here
$rw_test_mod_assembly_dest = "";

#Clean up library mods, the dll's already exist because of mod dependencies set in About.xml
$delete = @("Harmony", "0Harmony", "HugsLib")

foreach ($item in $delete) { 
    $path = "../assemblies/${item}.dll"
	Write-Output "deleting $path"
    if (Test-Path $path){
        Remove-Item $path
    }
}

if ($rw_test_mod_location)
{
	robocopy "../assemblies/" $rw_test_mod_assembly_dest /E
}


