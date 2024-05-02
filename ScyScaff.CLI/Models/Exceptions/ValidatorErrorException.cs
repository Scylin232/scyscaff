namespace ScyScaff.CLI.Models.Exceptions;

public class ValidatorErrorException(string errorMessage) : Exception(errorMessage);