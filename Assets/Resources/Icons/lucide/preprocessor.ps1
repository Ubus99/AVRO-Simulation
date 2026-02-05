# from ChatGPT
# Make SVGs importable by Unity Vector Graphics
# - Replaces currentColor with white
# - Strips <style> blocks (Unity ignores CSS)
# - Ensures explicit fill/stroke colors

Get-ChildItem -Filter *.svg | ForEach-Object {
    $path = $_.FullName
    $svg = Get-Content $path -Raw

    # Remove <style>...</style>
    $svg = [regex]::Replace(
            $svg,
            '<style[\s\S]*?</style>',
            '',
            [System.Text.RegularExpressions.RegexOptions]::IgnoreCase
    )

    # Replace currentColor with white (best for UI Toolkit tinting)
    $svg = $svg -replace 'currentColor', '#FFFFFF'

    # Ensure fill defaults (if missing or CSS-based)
    $svg = [regex]::Replace(
            $svg,
            '(?<!fill=)"none"',
            '"none"',
            'IgnoreCase'
    )

    # Optional: force stroke colors if needed
    # $svg = $svg -replace 'stroke="[^"]*"', 'stroke="#FFFFFF"'

    Set-Content -Path $path -Value $svg -Encoding UTF8
    Write-Host "Processed $($_.Name)"
}

Write-Host "All SVGs processed for Unity import."
