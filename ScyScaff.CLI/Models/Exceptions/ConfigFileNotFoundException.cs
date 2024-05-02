namespace ScyScaff.CLI.Models.Exceptions;

public class ConfigFileNotFoundException(string errorMessage) : Exception(errorMessage);