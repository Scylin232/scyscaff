using ScyScaff.CLI.Utils.Builder;

namespace ScyScaff.Tests.Builder;

public class FilePathParserTests
{
    [Fact]
    public void ReplacePatterns_SuccessfullyReplaced_Test()
    {
        // Arrange
        string filePath = "SCF_PIPE SCF_DOT";

        // Act
        string filePathResult = FilePathParser.ReplacePatterns(filePath);

        // Assert
        string expectedResult = "| .";
        Assert.Equal(expectedResult, filePathResult);
    }

    [Fact]
    public void ReplacePatterns_StringWithoutSpecialCharactersIsCanceled_Test()
    {
        // Arrange
        string filePath = "Test";
        
        // Act
        string filePathResult = FilePathParser.ReplacePatterns(filePath);
        
        // Assert
        string expectedResult = "Test";
        Assert.Equal(expectedResult, filePathResult);
    }
}