namespace ScyScaff.Core.Models.Exceptions;

public class ConfigFileNotFoundException(string errorMessage) : Exception(errorMessage);