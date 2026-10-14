# PowerShell 7+. Interactive only: password never appears in command arguments or output.
param([string]$OwnerKey)
$ErrorActionPreference = 'Stop'
if ($PSVersionTable.PSVersion.Major -lt 7) { throw 'Use PowerShell 7 or later.' }
if ($OwnerKey) {
    $readingGuid = [Guid]::ParseExact($OwnerKey, 'D')
    if ($readingGuid -eq [Guid]::Empty -or $readingGuid.ToString('D') -cne $OwnerKey) { throw 'OwnerKey must be a canonical nonempty UUID.' }
} else {
    $OwnerKey = [Guid]::NewGuid().ToString('D')
    Write-Warning 'Use the generated OwnerKey only for first setup. Never replace an existing library OwnerKey.'
}
$readingRuntime = Get-ChildItem (Join-Path $env:ProgramFiles 'dotnet/shared/Microsoft.AspNetCore.App') -Directory |
    Where-Object Name -Like '8.*' | Sort-Object { [Version]$_.Name } -Descending | Select-Object -First 1
if (-not $readingRuntime) { throw '.NET 8 ASP.NET Core shared runtime is required.' }
Add-Type -Path (Join-Path $readingRuntime.FullName 'Microsoft.Extensions.Options.dll')
Add-Type -Path (Join-Path $readingRuntime.FullName 'Microsoft.AspNetCore.Cryptography.KeyDerivation.dll')
Add-Type -Path (Join-Path $readingRuntime.FullName 'Microsoft.Extensions.Identity.Core.dll')
$readingSecret = Read-Host 'Reading access password (at least 4 characters)' -AsSecureString
$readingPointer = [Runtime.InteropServices.Marshal]::SecureStringToBSTR($readingSecret)
try {
    $readingPlaintext = [Runtime.InteropServices.Marshal]::PtrToStringBSTR($readingPointer)
    if ($readingPlaintext.Length -lt 4 -or $readingPlaintext.Length -gt 1024) { throw 'Password length must be 4..1024 characters.' }
    $readingHasher = [Microsoft.AspNetCore.Identity.PasswordHasher[object]]::new($null)
    $readingHash = $readingHasher.HashPassword([object]::new(), $readingPlaintext)
    @{ ReadingLibrary = @{ OwnerKey = $OwnerKey; AccessPasswordHash = $readingHash; SessionDays = 30; AbsoluteSessionDays = 90 } } |
        ConvertTo-Json -Depth 3
} finally {
    [Runtime.InteropServices.Marshal]::ZeroFreeBSTR($readingPointer)
    $readingPlaintext = $null
    $readingSecret.Dispose()
}
