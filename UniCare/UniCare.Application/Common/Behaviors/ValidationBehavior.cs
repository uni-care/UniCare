using FluentValidation;
using MediatR;
using System.ComponentModel.DataAnnotations;
using UniCare.Application.Common;

namespace UniCare.Application.Common.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        => _validators = validators;

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (!_validators.Any())
            return await next();

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .ToList();

        if (failures.Count == 0)
            return await next();

        // Try to create a Result<T>.ValidationFailure response; otherwise throw.
        var responseType = typeof(TResponse);
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var innerType = responseType.GetGenericArguments()[0];
            var resultType = typeof(Result<>).MakeGenericType(innerType);
            var errorMessages = failures.Select(f => f.ErrorMessage);

            var failureMethod = resultType.GetMethod(nameof(Result<object>.ValidationFailure))!;
            return (TResponse)failureMethod.Invoke(null, [errorMessages])!;
        }

        throw new ValidationException(failures);
    }
}
