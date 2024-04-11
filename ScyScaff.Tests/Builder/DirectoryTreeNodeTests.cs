using System.IO.Abstractions.TestingHelpers;
using ScyScaff.Core.Models.Builder;
using ScyScaff.Core.Utils.Builder;

namespace ScyScaff.Tests.Builder;

public class DirectoryTreeNodeTests
{
    [Fact]
    public void GetDirectoryTree_DirectoryTreeNodesBuiltCorrectly_Test()
    {
        // Arrange
        MockFileSystem mockFileSystem = new();
        
        mockFileSystem.AddDirectory("./Builder/Materials/DirectoryTreeTest/");
        mockFileSystem.AddFile("./Builder/Materials/DirectoryTreeTest/_", new MockFileData(string.Empty));
        
        mockFileSystem.AddDirectory("./Builder/Materials/DirectoryTreeTest/DirectoryTreeTest_1/");
        mockFileSystem.AddFile("./Builder/Materials/DirectoryTreeTest/DirectoryTreeTest_1/_", new MockFileData(string.Empty));

        mockFileSystem.AddDirectory("./Builder/Materials/DirectoryTreeTest/DirectoryTreeTest_1/DirectoryTreeTest_1_2/");
        mockFileSystem.AddFile("./Builder/Materials/DirectoryTreeTest/DirectoryTreeTest_1/DirectoryTreeTest_1_2/_", new MockFileData(string.Empty));

        mockFileSystem.AddDirectory("./Builder/Materials/DirectoryTreeTest/DirectoryTreeTest_2/");
        mockFileSystem.AddFile("./Builder/Materials/DirectoryTreeTest/DirectoryTreeTest_2/_", new MockFileData(string.Empty));

        // Act
        DirectoryTreeNode directoryTreeNode = DirectoryTree.GetDirectoryTree(mockFileSystem, "./Builder/Materials/DirectoryTreeTest/");

        // Assert
        Assert.Equal(3, directoryTreeNode.Children.Count);
        Assert.Equal(2, directoryTreeNode.Children[0].Children.Count);
        Assert.Single(directoryTreeNode.Children[0].Children[0].Children);
    }
}