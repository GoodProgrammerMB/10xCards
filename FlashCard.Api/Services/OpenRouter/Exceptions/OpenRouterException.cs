namespace FlashCard.Api.Services.OpenRouter.Exceptions;

public class OpenRouterException : Exception
{
    public OpenRouterException(string message) : base(message)
    {
    }

    public OpenRouterException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class OpenRouterAuthenticationException : OpenRouterException
{
    public OpenRouterAuthenticationException(string message) : base(message)
    {
    }
}

public class OpenRouterValidationException : OpenRouterException
{
    public OpenRouterValidationException(string message) : base(message)
    {
    }

    public OpenRouterValidationException(string message, Exception innerException) : base(message, innerException)
    {
    }
}

public class OpenRouterCommunicationException : OpenRouterException
{
    public int StatusCode { get; }

    public OpenRouterCommunicationException(string message, int statusCode) : base(message)
    {
        StatusCode = statusCode;
    }
} 