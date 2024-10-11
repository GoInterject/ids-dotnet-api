# Recursively builds toc.yml file based on source/pages folder

# Get the directory of the current script
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path

$baseDir = "source/pages"

# Set the path to the pages directory
$pagesDir = Join-Path $scriptDir $baseDir

# Check if the pages directory exists
if (-Not (Test-Path $pagesDir)) {
    Write-Host "The specified directory does not exist: $pagesDir"
    exit
}

# Get all .md files in the pages directory recursively
$files = Get-ChildItem -Path $pagesDir -Filter *.md -Recurse -File

# Initialize a string to hold the YAML content
$yamlContent = "---`n" # Start the YAML file with a header

# Group files by their subfolder path
$groupedFiles = $files | Group-Object { $_.DirectoryName }

# Loop through each group (subfolder) and create a YAML entry
foreach ($group in $groupedFiles) {
    # Get the folder title and path
    $folderTitle = Split-Path $group.Name -Leaf # Get only the last part of the path
    $folderTitle2 = $folderTitle -replace "_", " "
    $folderTitle3 = (Get-Culture).TextInfo.ToTitleCase($folderTitle2)

    $folderPath = $group.Name.Replace('\', '/').Substring($scriptDir.Length + $baseDir.Length + 1)

    # Add the section title and path for the subfolder
    $yamlContent += "- sectiontitle: '$folderTitle3'`n"
    $yamlContent += "  path: '$folderPath'`n"
    $yamlContent += "  section:`n"

    # Loop through each file in the subfolder
    foreach ($file in $group.Group) {
        # Get the relative path and replace backslashes with slashes
        # $relativePath = $file.FullName.Replace('\', '/').Substring($scriptDir.Length + $pagesDir.Length)
        $relativePath = $file.FullName.Replace('\', '/').Substring($scriptDir.Length + $baseDir.Length + 1)
        # $relativePath = $file.FullName.Replace('\', '/') -replace [regex]::Escape("$($pagesDir)/"), ""

        # Append the YAML entry for the file
        $yamlContent += "  - title: '$($file.BaseName)'`n"
        $yamlContent += "    path: '$relativePath'`n"
    }
}

# Check if any files were found and output to toc.yaml
if ($files.Count -gt 0) {
    # Output the YAML content to toc.yaml in the same directory as the script
    $yamlOutputPath = Join-Path $scriptDir "toc.yaml"
    $yamlContent | Out-File -FilePath $yamlOutputPath -Encoding utf8

    # Output the path of the generated toc.yaml
    Write-Host "toc.yaml has been generated at: $yamlOutputPath"
} else {
    Write-Host "No .md files found in the specified directory."
}
