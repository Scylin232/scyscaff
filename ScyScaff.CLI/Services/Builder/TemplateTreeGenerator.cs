﻿using System.IO.Abstractions;
using System.Text;
using DiffPlex.DiffBuilder;
using DiffPlex.DiffBuilder.Model;
using Scriban;
using ScyScaff.CLI.Models.Builder;
using ScyScaff.CLI.Models.Exceptions;
using ScyScaff.CLI.Utils.Builder;
using ScyScaff.CLI.Utils.Constants;
using ScyScaff.Core.Models.Events;
using ScyScaff.Core.Models.Parser;

namespace ScyScaff.CLI.Services.Builder;

// This class is responsible for generating files and directories based on a template tree.
public static class TemplateTreeGenerator
{
    // Generates files and directories based on the provided template tree.
    public static async Task GenerateFromTree(TreeGenerationContext generationContext, string workingDirectory)
    {
        // Create a directory for the entity based on project name and entity name.
        IDirectoryInfo entityDirectory = generationContext.FileSystem.Directory.CreateDirectory(generationContext.FileSystem.Path.Combine(workingDirectory, $"{generationContext.Config.ProjectName}.{generationContext.EntityName}"));

        // Check if the template plugin implements the IGenerationEvents interface.
        IGenerationEvents? generationEvents = generationContext.TemplatePlugin as IGenerationEvents;

        // Invoke OnServiceGenerationStarted event if supported by the template plugin.
        if (generationEvents is not null)
            await generationEvents.OnGenerationStarted(entityDirectory, generationContext.ScaffolderEntity);

        // Get the path to the template tree.
        string templateTreePath = generationContext.PathGatherer.GetPluginTemplateTreePath(generationContext.TemplatePlugin);
        
        // Generate a directory tree nodes in template tree directory.
        DirectoryTreeNode rootTemplateTreeNode = DirectoryTree.GetDirectoryTree(generationContext.FileSystem, templateTreePath);

        // Set some properties in the generation context for use in template processing.
        generationContext.EntityDirectory = entityDirectory.FullName;
        generationContext.TemplateTreePathLength = templateTreePath.Length;

        // Recursively parse and generate files based on the template tree.
        ParseServiceTree(rootTemplateTreeNode, generationContext);

        // Invoke OnServiceGenerationEnded event if supported by the template plugin.
        if (generationEvents is not null)
            await generationEvents.OnGenerationEnded(entityDirectory, generationContext.ScaffolderEntity);
    }

    // Recursively parses the template tree and generates files and directories accordingly.
    private static void ParseServiceTree(DirectoryTreeNode treeNode, TreeGenerationContext context)
    {
        // If the path contains a model iterator placeholder, generate files for each model.
        if (treeNode.Path.Contains("[...modelIterator...]"))
        {
            // Get model template directories and files.
            IEnumerable<string> modelTemplateDirectories = context.FileSystem.Directory
                .GetDirectories(context.FileSystem.Path.GetDirectoryName(treeNode.Path)!, "*.*", SearchOption.AllDirectories)
                .ToList();
            
            IEnumerable<string> modelTemplateFiles = context.FileSystem.Directory
                .GetFiles(context.FileSystem.Path.GetDirectoryName(treeNode.Path)!, "*.*", SearchOption.AllDirectories)
                .Where(f => f.EndsWith(".liquid"))
                .ToList();

            // If a service is specified in the context, we will use its models, otherwise we will use the models of all services.
            List<ModelContext> modelsContexts = new();

            // If the service was specified, then add all its models and assign them the current entityName from the context
            if (context.ScaffolderEntity is ScaffolderService contextService)
                modelsContexts.AddRange(contextService.Models.Select(model => new ModelContext { Model = model, BelongedEntityName = context.EntityName }));
            // If the service was not specified, then add models from all services and take the name from the configuration directly
            else
                modelsContexts.AddRange(from service in context.Config.Services from model in service.Value.Models select new ModelContext { Model = model, BelongedEntityName = service.Key });
            
            foreach (ModelContext modelContext in modelsContexts)
            {
                foreach (string modelTemplateDirectory in modelTemplateDirectories)
                    GenerateServiceFile(modelTemplateDirectory, context, modelContext);

                foreach (string modelTemplateFile in modelTemplateFiles)
                    GenerateServiceFile(modelTemplateFile, context, modelContext);
            }
        }

        // If the path ends with ".liquid" and is not in an iterable directory, generate the file.
        if (treeNode.Path.EndsWith(".liquid"))
        {
            bool isInIterableDirectory = context.FileSystem.Directory
                .GetFiles(context.FileSystem.Path.GetDirectoryName(treeNode.Path)!)
                .Any(directory => directory.Contains("[..."));

            if (!isInIterableDirectory)
                GenerateServiceFile(treeNode.Path, context);
        }

        // If the node represents a directory and does not have templated name, create it in the entity directory.
        if (context.FileSystem.File.GetAttributes(treeNode.Path).HasFlag(FileAttributes.Directory) && !treeNode.Path.Contains("{{"))
            context.FileSystem.Directory.CreateDirectory(context.FileSystem.Path.Combine(context.EntityDirectory, treeNode.Path[context.TemplateTreePathLength..]));

        // Recursively process child nodes.
        foreach (DirectoryTreeNode treeChild in treeNode.Children.Where(treeChild => !treeChild.Path.Contains("{{")))
            ParseServiceTree(treeChild, context);
    }

    // Generates a service file based on the provided template file and context.
    private static void GenerateServiceFile(string filePath, TreeGenerationContext context, ModelContext? modelContext = default)
    {
        // Replace specific patterns to appropriate symbols.
        // NOTE: Used for symbols that can't be used in filenames, but can be used in parser, like: . (Dot), | (Pipe), etc.
        string parsedFilePath = FilePathParser.ReplacePatterns(filePath);

        // List all parameters that we use for our templates.
        object templateParameters = new
        {
            context.Config,
            Entity = context.ScaffolderEntity,
            context.EntityName,
            modelContext?.Model,
            ModelEntityName = modelContext?.BelongedEntityName,
            context.ComposeServices
        };
        
        // Parse the file path as a Scriban template to get the actual file name.
        Template fileNameTemplate = Template.Parse(parsedFilePath);
        string fileNameResult = fileNameTemplate.Render(templateParameters);
        
        // Build the new file path in the entity directory.
        string newFilePath = context.FileSystem.Path.Combine(context.EntityDirectory, fileNameResult[context.TemplateTreePathLength..]);

        // If the original file path represents a directory, create it and stop the function.
        if (context.FileSystem.File.GetAttributes(filePath).HasFlag(FileAttributes.Directory))
        {
            context.FileSystem.Directory.CreateDirectory(newFilePath);
            return;
        }

        // Parse the file content using Scriban.
        Template fileContentTemplate = Template.Parse(context.FileSystem.File.ReadAllText(filePath));
        string fileContentResult = fileContentTemplate.Render(templateParameters);

        // If the rendered result is empty, stop writing the file.
        if (fileContentResult.Length <= 0) return;
        
        // Determine target file path, trimming ".liquid" from the file name.
        string targetFilePath = newFilePath[..^7];

        // If the file exists and its content is different from the render result, we should start the process of adding new lines rather than replacing them all.
        if (context.FileSystem.File.Exists(targetFilePath))
        {
            // Read the data from an existing file for further verification.
            string targetFileContent = context.FileSystem.File.ReadAllText(targetFilePath);

            // If the rendered result and the existing file content are the same, we stop the function.
            if (targetFileContent == fileContentResult) return;
            
            // Check if add-mode is enabled, if not, throw an exception.
            if (!context.IsAddModeEnabled)
                throw new AddModeNotEnabledException(Messages.AddModeNotEnabledError);

            // Call the function to add new lines if everything is good to go.
            AddNewLines(context.FileSystem, targetFilePath, fileContentResult);

            // Stop the function.
            return;
        }
        
        // Write the processed content to the new file.
        context.FileSystem.File.WriteAllText(targetFilePath, fileContentResult);
    }

    // Only add new lines to the target file based on the content we generated.
    private static void AddNewLines(IFileSystem fileSystem, string targetFilePath, string fileContentResult)
    {
        // Read the entire content of the file and compare it with the content we processed. If it is the same, we should exit the function.
        string targetFileContent = fileSystem.File.ReadAllText(targetFilePath);

        if (targetFileContent == fileContentResult) return;

        // Create textual diff model between existing file content and new one.
        DiffPaneModel diff = InlineDiffBuilder.Diff(fileContentResult, targetFileContent);
        // Create a StringBuilder, so that we will write string-lines in a loop.
        StringBuilder outputBuilder = new();

        // Add a line to StringBuilder if it was: Deleted (Added by a new version, since it is taken as the original), unchanged, inserted (custom user code).
        foreach (DiffPiece? line in diff.Lines.Where(line => line.Type == ChangeType.Deleted || line.Type == ChangeType.Unchanged || line.Type == ChangeType.Inserted))
            outputBuilder.Append($"{line.Text}\n");

        // Write StringBuilder to file.
        fileSystem.File.WriteAllText(targetFilePath, outputBuilder.ToString());
    }
}