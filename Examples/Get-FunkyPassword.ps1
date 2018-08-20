#This is an example how to generate funky passwords in PowerShell

function Get-FunkyPassword{
    [cmdletbinding()]
    param
    (
        [int] $minlen = 10,
        [string] $lang = "Random",
        [bool] $asciionly = $true
    )

    $response = Invoke-RestMethod -Uri "https://funkypass.interkreacja.pl/api/GeneratePassword?minlen=$minlen&lang=$lang&asciionly=$asciionly"

    Write-Verbose "About your password:" 
    Write-Verbose ($response | Format-List -Property password_length,password_entropy | Out-String)
    Write-Verbose "About the language:" 
    Write-Verbose ($response.language | Format-List | Out-String)
 
    return $response.password
}


#get completely random password
$funkypassword = Get-FunkyPassword 
Write-Host $funkypassword

#choose language of your password
$funkypassword = Get-FunkyPassword -lang "en" 
Write-Host $funkypassword

#get password containing diacritics and show information about the password
$funkypassword = Get-FunkyPassword -minlen 10 -lang "pl" -asciionly $false -verbose
Write-Host $funkypassword

