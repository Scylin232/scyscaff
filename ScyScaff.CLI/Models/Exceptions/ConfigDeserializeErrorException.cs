namespace ScyScaff.Core.Models.Exceptions;

public class ConfigDeserializeErrorException(string errorMessage) : Exception(errorMessage);