namespace ScyScaff.Core.Utils.Builder;

public static class FilePathParser
{
    private static Dictionary<string, string> _replacements = new()
    {
        {"SCF_PIPE", "|"},
        {"SCF_DOT", "."}
    };

    public static string ReplacePatterns(string filePath)
    {
        if (!_replacements.Keys.Any(filePath.Contains))
            return filePath;
        
        string parsedFilePath = filePath;

        foreach (KeyValuePair<string, string> replacement in _replacements)
            parsedFilePath = parsedFilePath.Replace(replacement.Key, replacement.Value);
        
        return parsedFilePath;
    }
}