public class ValidationException : Exception
{
     public List<string> Errors { get; }

    public ValidationException(string message, List<string> errors)
        : base(message)
    {
        Errors = errors ?? new List<string>();
    }

    public ValidationException(string message, string error)
        : base(message)
    {
        Errors = new List<string> { error };
    }

    public ValidationException(string error)
        : base("Validation failed")
    {
        Errors = new List<string> { error };
    }
}
