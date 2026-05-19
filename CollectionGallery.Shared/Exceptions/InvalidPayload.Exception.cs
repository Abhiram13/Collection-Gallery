namespace CollectionGallery.Shared.Exceptions;

/// <summary>
/// Raised when a user given payload is invalid
/// </summary>
public class InvalidPayloadException : Exception
{
    /// <inheritdoc cref="InvalidPayloadException"/>
    public InvalidPayloadException() { }
    
    /// <inheritdoc cref="InvalidPayloadException"/>
    /// <param name="message">Exception message that contains reason</param>
    public InvalidPayloadException(string message) : base(message) {  }
}