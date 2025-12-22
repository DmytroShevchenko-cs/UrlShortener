namespace UrlShortener.Shared.Common.CQRS;

using MediatR;

public interface IQuery<out T> : IRequest<T>
{
}