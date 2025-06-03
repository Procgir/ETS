using ElectronicTestSystem.Domain.Abstractions;
using MediatR;

namespace ElectronicTestSystem.Application.Abstractions.Messaging;

public interface IQuery<TResponse> : IRequest<Result<TResponse>>
{
    
}