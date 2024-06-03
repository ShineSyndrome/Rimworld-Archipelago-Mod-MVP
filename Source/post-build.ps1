#Put your local test mod assembly location here
$rw_test_mod_dest = "C:\RIMWorld_Debug\Mods\Rimworld-Archipelago-Mod-MVP";

$assembly_path = $rw_test_mod_dest + "\Assemblies"
$def_path = $rw_test_mod_dest + "\1.4\Defs"

#Clean up library mods, the dll's already exist because of mod dependencies set in About.xml
$delete = @("Harmony", "0Harmony", "HugsLib")

foreach ($item in $delete) { 
    $path = "../assemblies/${item}.dll"
	Write-Output "deleting $path"
    if (Test-Path $path){
        Remove-Item $path
    }
}

#Voodoo. Required to stop errors from .dll's being loaded in the wrong order.
$items = Get-ChildItem -Path ../assemblies
$order = @("Harmony", "Newtonsoft", "websocket", "Archipelago.MultiClient", "RimworldArchipelago")
Write-Output "Assemblies contains the following: $items" 
foreach ($item in $items) { 
    for ( $index = 0; $index -lt $order.count; $index++) {
        if ($item.Name.StartsWith($order[$index])) {
            $item | Rename-Item -NewName { "$index"+"_"+$_.Name };
        }
    }
}

if ($rw_test_mod_dest)
{
	robocopy "../assemblies/" $assembly_path /E
	
	$old_defs = Get-ChildItem -Path $def_path
	foreach ($item in $old_defs) {
        $path = $def_path + "/${item}"
		Remove-Item $path -recurse
	}
	
	robocopy "../1.4/defs/" $def_path /E
}