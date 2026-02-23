# plantuml.ps1
# Overwrites include.puml files so that configured PlantUML commands are placed
# immediately after the @startuml line (or wraps the file if no @startuml present).

# ---------------- CONFIG ----------------

# Directories to scan / generate UML for (easy to extend)
$ScanDirectories = @(
    @{ Source = "..\Scripts\UI"; Target = ".\UML\Scripts\UI" },
    @{ Source = "..\Scripts\Gameplay"; Target = ".\UML\Scripts\Gameplay" },
    @{ Source = "..\Scenes\Simulation"; Target = ".\UML\Simulation" }
)

# PlantUML directives to ensure appear directly after @startuml in include.puml
# Add or remove lines here; each item is a PlantUML statement (not a comment).
$PlantUmlHeaderLines = @(
    "hide empty members",,
    "hide interface"
    
    "remove MonoBehaviour",
    "remove ScriptableObject",
    "remove VisualTreeAsset",
    
    "remove VisualElement",
    "remove Texture2D",
    "remove VectorImage",
    "remove Image",
    "remove Label"
)

# Additional flags passed to puml-gen
$flags = @("-dir", "-createAssociation", "-addPackageTags")

# --------------- HELPERS ----------------

function Ensure-Directory
{
    param([string]$Path)
    if (-not (Test-Path -LiteralPath $Path))
    {
        New-Item -ItemType Directory -Path $Path -Force | Out-Null
    }
}

function Write-FileLines
{
    param(
        [string]$Path,
        [string[]]$Lines
    )
    Ensure-Directory -Path (Split-Path -Path $Path -Parent)
    Set-Content -LiteralPath $Path -Value ($Lines -join "`n") -Encoding UTF8
}

function Insert-PlantUmlHeader
{
    param(
        [string]$FilePath,
        [string[]]$HeaderLines
    )

    if (-not (Test-Path -LiteralPath $FilePath))
    {
        # Create a new file wrapped with @startuml / @enduml and header lines
        $new = @()
        $new += "@startuml"
        $new += $HeaderLines
        $new += ""
        $new += "@enduml"
        Write-FileLines -Path $FilePath -Lines $new
        return
    }

    # Read as array of lines
    $lines = Get-Content -LiteralPath $FilePath -ErrorAction Stop

    # Find the first @startuml line (case-insensitive)
    $startIdx = -1
    for ($i = 0; $i -lt $lines.Count; $i++) {
        if ($lines[$i] -match '^\s*@startuml\b')
        {
            $startIdx = $i
            break
        }
    }

    if ($startIdx -ge 0)
    {
        # Determine a short window after @startuml to search for existing header lines
        $windowStart = $startIdx + 1
        $windowEnd = [Math]::Min($lines.Count - 1, $startIdx + 40)  # check next up to 40 lines
        $window = if ($windowStart -le $windowEnd)
        {
            $lines[$windowStart..$windowEnd]
        }
        else
        {
            @()
        }

        # If all header lines already present in the window (in any order), skip insertion
        $allPresent = $true
        foreach ($h in $HeaderLines)
        {
            if (-not ($window | Where-Object { $_ -eq $h }))
            {
                $allPresent = $false
                break
            }
        }
        if ($allPresent)
        {
            return
        }

        # Insert header lines immediately after the @startuml line
        $before = if ($startIdx -ge 0)
        {
            $lines[0..$startIdx]
        }
        else
        {
            @()
        }
        $after = if ($startIdx + 1 -le $lines.Count - 1)
        {
            $lines[($startIdx + 1) .. ($lines.Count - 1)]
        }
        else
        {
            @()
        }

        # Build new content: before + header + blank line + after
        $newContent = @()
        $newContent += $before
        $newContent += $HeaderLines
        $newContent += ""
        $newContent += $after

        Write-FileLines -Path $FilePath -Lines $newContent
    }
    else
    {
        # No @startuml present: wrap whole file with @startuml / header / original content / @enduml
        $hasEnd = ($lines | Where-Object { $_ -match '^\s*@enduml\b' }).Count -gt 0
        $new = @()
        $new += "@startuml"
        $new += $HeaderLines
        $new += ""
        $new += $lines
        if (-not $hasEnd)
        {
            $new += ""
            $new += "@enduml"
        }
        Write-FileLines -Path $FilePath -Lines $new
    }
}

# ----------------- MAIN ------------------

# Clean output directory if present
Remove-Item .\UML -Recurse -Force -ErrorAction SilentlyContinue

foreach ($entry in $ScanDirectories)
{
    $src = $entry.Source
    $dst = $entry.Target

    # Ensure destination exists before running generator (generator may create it itself)
    Ensure-Directory -Path $dst

    # Run puml-gen (assumed available in environment)
    puml-gen $src $dst $flags

    # Target file name is "include.puml" (singular) as requested
    $includeFile = Join-Path -Path $dst -ChildPath "include.puml"

    if (Test-Path -LiteralPath $includeFile)
    {
        # Insert header between @startuml and rest (or wrap if no @startuml)
        Insert-PlantUmlHeader -FilePath $includeFile -HeaderLines $PlantUmlHeaderLines
    }
    else
    {
        # If generator did not create the file, create a minimal include.puml wrapped with header
        Insert-PlantUmlHeader -FilePath $includeFile -HeaderLines $PlantUmlHeaderLines
    }
}
