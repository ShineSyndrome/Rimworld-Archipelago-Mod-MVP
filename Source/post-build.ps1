#Put your local test mod assembly location here
$rw_test_mod_assembly_dest = "C:\RIMWorld_Debug\Mods\Rimworld-Archipelago-Mod-MVP\Assemblies";

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

if ($rw_test_mod_assembly_dest)
{
	robocopy "../assemblies/" $rw_test_mod_assembly_dest /E
}


