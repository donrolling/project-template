
$json = (Get-Content 'CodeGenerator\config.json') -join "`n" | ConvertFrom-Json
$outputDirectory = $json | Select -expand OutputDirectory

$destinationPath = "C:\Projects\IdentityServer\"

$path = "$outputDirectory\Business\"
Copy-Item -Path $path -Destination $destinationPath -Recurse -Force

$path = "$outputDirectory\Data\"
Copy-Item -Path $path -Destination $destinationPath -Recurse -Force

$path = "$outputDirectory\Tests\"
Copy-Item -Path $path -Destination $destinationPath -Recurse -Force

$server = "971JT039H2\DROLLING"
$database = "Identity"
$command = "CodeGenerator\Scripts\runSQL.ps1"
$path = "$outputDirectory\Database\Drop"
Invoke-Expression "$command -server $server -database $database -path $path"

$path = "$outputDirectory\Database\Functions"
Invoke-Expression "$command -server $server -database $database -path $path"

$path = $outputDirectory + '\Database\Stored` Procedures'
Invoke-Expression "$command -server $server -database $database -path $path"

Read-Host -Prompt "Press Enter to exit"