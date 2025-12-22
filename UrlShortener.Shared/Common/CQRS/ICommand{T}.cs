namespace UrlShortener.Shared.Common.CQRS;

using MediatR;

public interface ICommand<out T> : IRequest<T>
{
}