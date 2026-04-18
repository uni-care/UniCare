using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UniCare.Application.Common;
using UniCare.Application.Common.cqrs;
using UniCare.Domain.Aggregates.TransactionHandover;

namespace UniCare.Application.TransactionHandover.Commands.GenerateHandover
{
    public sealed class GenerateHandoverCommandHandler
     : ICommandHandler<GenerateHandoverCommand, Result<GenerateHandoverResult>>
    {
        private readonly ITransactionHandoverRepository _repository;
        private readonly IPinGeneratorService _pinGenerator;

        // PINs expire after 30 minutes — enough time for in-person meetups
        private static readonly TimeSpan PinLifetime = TimeSpan.FromMinutes(30);

        public GenerateHandoverCommandHandler(
            ITransactionHandoverRepository repository,
            IPinGeneratorService pinGenerator)
        {
            _repository = repository;
            _pinGenerator = pinGenerator;
        }

        public async Task<Result<GenerateHandoverResult>> Handle(
            GenerateHandoverCommand command,
            CancellationToken cancellationToken)
        {
            // Invalidate any existing pending code for same transaction + type
            var existing = await _repository.GetPendingByTransactionAndTypeAsync(
                command.TransactionId, command.Type, cancellationToken);

            if (existing is not null)
            {
                existing.Expire();
                await _repository.UpdateAsync(existing, cancellationToken);
            }

            var rawPin = _pinGenerator.GeneratePin();
            var pinHash = _pinGenerator.HashPin(rawPin);
            var expiresAt = DateTime.UtcNow.Add(PinLifetime);

            var handover = UniCare.Domain.Aggregates.TransactionHandover.TransactionHandover.Create(
                transactionId: command.TransactionId,
                type: command.Type,
                pin: rawPin,
                tokenHash: pinHash,
                generatedForUserId: command.GeneratedForUserId,
                verifiedByUserId: command.VerifiedByUserId,
                expiresAt: expiresAt
            );

            await _repository.AddAsync(handover, cancellationToken);

            // QR payload — the mobile app encodes this JSON string into a QR image.
            // On scan, it extracts the PIN and calls the verify endpoint automatically.
            var qrPayload = JsonSerializer.Serialize(new
            {
                handoverId = handover.Id,
                transactionId = command.TransactionId,
                type = command.Type.ToString(),
                pin = rawPin
            });

            return Result<GenerateHandoverResult>.Success(new GenerateHandoverResult(
                HandoverId: handover.Id,
                Pin: rawPin,
                QrPayload: qrPayload,
                Type: command.Type,
                ExpiresAt: expiresAt
            ));
        }
    }

}
