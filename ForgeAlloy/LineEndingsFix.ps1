Get-ChildItem -Path .\Unity\Assets\ForgeNetworking -Filter *.cs -Recurse -File | ForEach-Object {
    # If contains UNIX line endings, replace with Windows line endings
    if (Get-Content $_.FullName -Delimiter "`0" | Select-String "[^`r]`n")
    {
        $content = Get-Content $_.FullName
        $content | Set-Content $_.FullName
        Write-Output $_.FullName
    }
}