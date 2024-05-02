namespace ScyScaff.CLI.Models.Exceptions;

public class ConfigDeserializeErrorException(string errorMessage) : Exception(errorMessage);