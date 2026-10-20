# Backs up part of Documents\AgOpenGPS into a zip before installing. Called by AgOpenGPS.iss.
#
# Uses .NET ZipArchive instead of Compress-Archive: Compress-Archive in Windows PowerShell 5.1 is
# more than ten times slower on many small files (a field is a folder of small text files).
# Files are opened with FileShare.ReadWrite, so a log held open by a running AgOpenGPS or AgIO does
# not stop the backup, and a file that still cannot be read is skipped instead.
#
# -Include / -Exclude take top level names inside the source folder, e.g. -Include Fields.
# Exit code 0: zip written (or nothing to back up, then no zip is left behind). 1: failed.
param(
    [Parameter(Mandatory = $true)] [string]$Source,
    [Parameter(Mandatory = $true)] [string]$ZipPath,
    [string[]]$Include,
    [string[]]$Exclude
)

$ErrorActionPreference = 'Stop'
$ProgressPreference = 'SilentlyContinue'

$zip = $null
try {
    Add-Type -AssemblyName System.IO.Compression, System.IO.Compression.FileSystem
    $root = (Resolve-Path -LiteralPath $Source).ProviderPath.TrimEnd('\')

    $zip = [IO.Compression.ZipFile]::Open($ZipPath, [IO.Compression.ZipArchiveMode]::Create)
    $written = 0
    $skipped = 0

    foreach ($file in [IO.Directory]::EnumerateFiles($root, '*', [IO.SearchOption]::AllDirectories)) {
        $rel = $file.Substring($root.Length + 1)
        $top = $rel.Split('\')[0]
        if ($Include -and ($Include -notcontains $top)) { continue }
        if ($Exclude -and ($Exclude -contains $top)) { continue }

        try {
            $in = [IO.File]::Open($file, [IO.FileMode]::Open, [IO.FileAccess]::Read, [IO.FileShare]::ReadWrite)
            try {
                $entry = $zip.CreateEntry($rel.Replace('\', '/'), [IO.Compression.CompressionLevel]::Fastest)
                $entry.LastWriteTime = [IO.File]::GetLastWriteTime($file)
                $out = $entry.Open()
                try { $in.CopyTo($out) } finally { $out.Dispose() }
                $written++
            } finally { $in.Dispose() }
        } catch {
            $skipped++
        }
    }

    $zip.Dispose()
    $zip = $null

    if ($written -eq 0) { Remove-Item -LiteralPath $ZipPath -Force }
    Write-Output "Backed up $written file(s), skipped $skipped"
    exit 0
} catch {
    if ($zip) { $zip.Dispose() }
    if (Test-Path -LiteralPath $ZipPath) { Remove-Item -LiteralPath $ZipPath -Force -ErrorAction SilentlyContinue }
    Write-Output $_.Exception.Message
    exit 1
}
