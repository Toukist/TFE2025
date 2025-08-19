namespace Dior.Library.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message = "Non autorisé") : base(message) { }
    }

    public class ForbiddenException : Exception
    {
        public ForbiddenException(string message = "Accès interdit") : base(message) { }
    }

    public class ValidationException : Exception
    {
        public Dictionary<string, string[]> Errors { get; }
        
        public ValidationException(string message) : base(message) 
        {
            Errors = new Dictionary<string, string[]>();
        }
        
        public ValidationException(Dictionary<string, string[]> errors) : base("Erreurs de validation")
        {
            Errors = errors;
        }
    }

    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }
}