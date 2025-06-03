using ElectronicTestSystem.Domain.Abstractions;
using MediatR;

namespace ElectronicTestSystem.Application.Abstractions.Messaging;

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, Result<TResponse>>
    where TQuery : IQuery<TResponse>
{
    
}