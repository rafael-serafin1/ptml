namespace PTML

open System
open System.IO

module DirTree =
    type private Entry = {
        Name: string
        FullPath: string
        IsDirectory: bool
        IsReparsePoint: bool
    }

    let private getEntries (directory: DirectoryInfo) =
        Directory.EnumerateFileSystemEntries(directory.FullName)
        |> Seq.choose (fun path ->
            let attributes = File.GetAttributes(path)
            if (attributes &&& (FileAttributes.Hidden ||| FileAttributes.System)) <> enum 0 then
                None
            else
                Some {
                    Name = Path.GetFileName(path)
                    FullPath = path
                    IsDirectory = (attributes &&& FileAttributes.Directory) <> enum 0
                    IsReparsePoint = (attributes &&& FileAttributes.ReparsePoint) <> enum 0
                })
        |> Seq.sortWith (fun left right ->
            match left.IsDirectory, right.IsDirectory with
            | true, false -> -1
            | false, true -> 1
            | _ ->
                let insensitive = StringComparer.OrdinalIgnoreCase.Compare(left.Name, right.Name)
                if insensitive <> 0 then insensitive
                else StringComparer.Ordinal.Compare(left.Name, right.Name))
        |> Seq.toArray

    let render (path: string) (rootLimit: int) =
        if String.IsNullOrEmpty(path) then
            invalidArg (nameof path) "The directory tree path cannot be empty."
        if rootLimit < 0 then
            invalidArg (nameof rootLimit) "The directory tree root limit cannot be negative."

        let root = DirectoryInfo(path)
        let fullRootPath = Path.GetFullPath(path)
        let rootName =
            let name = Path.GetFileName(Path.TrimEndingDirectorySeparator(fullRootPath))
            if String.IsNullOrEmpty(name) then 
                fullRootPath 
            else 
                name

        let lines = ResizeArray<string>()
        lines.Add(rootName)

        let rec appendChildren (prefix: string) (directory: DirectoryInfo) =
            let entries = getEntries directory
            let displayedEntries = entries |> Array.truncate rootLimit
            let omittedCount = entries.Length - displayedEntries.Length

            for index in 0 .. displayedEntries.Length - 1 do
                let entry = displayedEntries.[index]
                let isLast = index = displayedEntries.Length - 1 && omittedCount = 0
                let branch = if isLast then "└── " else "├── "
                lines.Add(prefix + branch + entry.Name)

                if entry.IsDirectory && not entry.IsReparsePoint then
                    let childPrefix = prefix + (if isLast then "    " else "│   ")
                    appendChildren childPrefix (DirectoryInfo(entry.FullPath))

            if omittedCount > 0 then
                let omittedLabel = if omittedCount = 1 then "item omitido" else "itens omitidos"
                lines.Add(prefix + $"└── … ({omittedCount} {omittedLabel})")

        appendChildren "" root
        lines |> Seq.toList
