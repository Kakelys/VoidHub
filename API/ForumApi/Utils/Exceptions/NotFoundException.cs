
namespace ForumApi.Utils.Exceptions;

public class NotFoundException : BaseException
{
    public NotFoundException(string message) : base(message)
    {
    }

    public NotFoundException(string message, Exception inner) : base(message, inner)
    {
    }

    public NotFoundException() : base()
    {
    }
}