namespace ScyScaff.Core.Utils.Builder;

public static class FilePathParser
{
    private static readonly Dictionary<string, string> Replacements = new()
    {
        {"SCF_PIPE", "|"},
        {"SCF_DOT", "."}
    };

    public static string ReplacePatterns(string filePath)
    {
        if (!Replacements.Keys.Any(filePath.Contains))
            return filePath;
        
        string parsedFilePath = filePath;

        foreach (KeyValuePair<string, string> replacement in Replacements)
            parsedFilePath = parsedFilePath.Replace(replacement.Key, replacement.Value);
        
        return parsedFilePath;
    }
}