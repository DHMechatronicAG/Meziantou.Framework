pwsh --version

[Reflection.Assembly]::LoadWithPartialName('System.IO.Compression.FileSystem')

# $env:NuGetDirectory= Join-Path $PSScriptRoot "../artifacts/package/debug" -Resolve
$Generators = @("Meziantou.Framework.StronglyTypedId", "Meziantou.Framework.FastEnumToStringGenerator")
foreach ($Generator in $Generators) {
    Write-Host "Checking $Generator"

    $PackagePath = (Get-ChildItem $env:NuGetDirectory | Where-Object FullName -Match "$Generator.[0-9.-]+.nupkg").FullName
    $AnnotationPath = Join-Path $PSScriptRoot ".." "src" "$Generator.Annotations" -Resolve

    $Tfms = $(dotnet build --getProperty:TargetFrameworks $AnnotationPath).Split(";")

    $ZipFile = [IO.Compression.ZipFile]::OpenRead($PackagePath)
    $Entries = $ZipFile.Entries.FullName
    $ZipFile.Dispose()
    foreach ($Tfm in $Tfms) {
        # Check if there is an entry with a path that starts with "lib/$Tfm/"
        $Entry = $Entries | Where-Object { $_.StartsWith("lib/$Tfm/") }
        if (-not $Entry) {
            Write-Error "Package does not contain a lib/$Tfm/ entry"
            exit 1
        }
    }
}

# Ensure InlineSnapshot package contains the prompt folder
$PackagePath = (Get-ChildItem $env:NuGetDirectory | Where-Object FullName -Match "Meziantou.Framework.InlineSnapshotTesting.[0-9.-]+.nupkg").FullName
$ZipFile = [IO.Compression.ZipFile]::OpenRead($PackagePath)
$Entries = $ZipFile.Entries.FullName
$Entry = $Entries | Where-Object { $_.StartsWith("prompt/") }
if (-not $Entry) {
    Write-Error "Package does not contain a prompt/ entry"
    exit 1
}
$ZipFile.Dispose()

# General validation
Write-Host "Validating NuGet packages"
dotnet tool update Meziantou.Framework.NuGetPackageValidation.Tool --global --no-cache --add-source $env:NuGetDirectory
$files = Get-ChildItem "$env:NuGetDirectory/*" -Include *.nupkg

& meziantou.validate-nuget-package @files --excluded-rules "ReadmeMustBeSet,TagsMustBeSet" --excluded-rule-ids "52" --github-token=$env:GITHUB_TOKEN --only-report-errors
if ($LASTEXITCODE -ne 0) {
    exit 1
}