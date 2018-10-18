param([string]$server, [string]$database, [string]$path)
foreach ($f in Get-ChildItem -path $path -Filter *.sql){ 
    try {
        invoke-sqlcmd -ServerInstance $server -Database $database -InputFile $f.fullname 
    } catch {
        Write-Host "SQL Exception";
        $Error | format-list -force;
    }
}