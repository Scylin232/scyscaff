namespace ScyScaff.Core.Models.Exceptions;

public class ValidatorErrorException(string errorMessage) : Exception(errorMessage);