namespace ScyScaff.CLI.Utils.Builder;

public static class FilePathParser
{
    // Define a dictionary to hold the patterns to be replaced in file paths
    private static readonly Dictionary<string, string> Replacements = new()
    {
        // Key-value pairs representing the pattern to be replaced and its replacement
        {"SCF_PIPE", "|"},
        {"SCF_DOT", "."}
    };

    // Takes a file path as input and returns a string with replaced patterns
    public static string ReplacePatterns(string filePath)
    {
        // Check if the filePath contains any of the keys defined in Replacements
        // If not, return the original filePath without changes
        if (!Replacements.Keys.Any(filePath.Contains))
            return filePath;
        
        // Store the initial filePath in a new variable for manipulation
        string parsedFilePath = filePath;

        // Iterate through each key-value pair in the Replacements dictionary
        foreach (KeyValuePair<string, string> replacement in Replacements)
            // Replace occurrences of the dictionary's key in parsedFilePath with its corresponding value
            parsedFilePath = parsedFilePath.Replace(replacement.Key, replacement.Value);
        
        // Return the filePath with all replacements applied
        return parsedFilePath;
    }
}
