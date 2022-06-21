using Protocolor.Util;

namespace Protocolor;

public enum ErrorSeverity {
    /// <summary>
    /// It means you can probably do better, but its not that bad.
    /// </summary>
    Info,

    /// <summary>
    /// Pretty bad. Probably want to fix even if it technically compiles.
    /// </summary>
    Warning,
    
    /// <summary>
    /// Will not produce an output, but does not interrupt compilation so more errors can be found.
    /// </summary>
    Error,
    
    /// <summary>
    /// Error is so serious compilation is cancelled. We likely won't get anything useful from code with a fatal error.
    /// Still presented as Error to the end user.
    /// </summary>
    Fatal,
}

public class ErrorCode {
    public ErrorSeverity Severity { get; }
    public string Description { get; }
    public string Identifier { get; }

    public ErrorCode(string identifier, ErrorSeverity severity, string description) {
        Severity = severity;
        Description = description;
        Identifier = identifier;
    }

    public override string ToString() {
        return $"{Identifier}: {Description}";
    }
}

public class Error {
    public Rectangle Position { get; }
    public ErrorCode Code { get; }
    
    public string Message { get; }

    public Error(ErrorCode code, Rectangle position, string? customMessage = null) {
        Position = position;
        Code = code;
        Message = customMessage ?? code.Description;
    }

    public override string ToString() {
        return $"Error at position {Position}:\n{Message}";
    }
}
