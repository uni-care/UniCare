using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using UniCare.Application.TransactionHandover.DTOs;
using UniCare.Domain.Aggregates.TransactionHandover;

namespace UniCare.Application.TransactionHandover.UseCases
{
    public class GenerateHandoverUseCase
    {
        private readonly ITransactionHandoverRepository _repository;
        private readonly IPinGeneratorService _pinGenerator;

        // PINs expire after 30 minutes — long enough for meetups, short enough to limit risk
        private static readonly TimeSpan PinLifetime = TimeSpan.FromMinutes(30);

        public GenerateHandoverUseCase(
            ITransactionHandoverRepository repository,
            IPinGeneratorService pinGenerator)
        {
            _repository = repository;
            _pinGenerator = pinGenerator;
        }

        public async Task<GenerateHandoverResponse> ExecuteAsync(
            GenerateHandoverCommand command,
            CancellationToken ct = default)
        {
            // Expire any old pending code for the same transaction+type
            var existing = await _repository.GetPendingByTransactionAndTypeAsync(
                command.TransactionId, command.Type, ct);

            if (existing is not null)
                existing.Expire();

            // Generate new PIN and hash it for storage
            var rawPin = _pinGenerator.GeneratePin();
            var pinHash = _pinGenerator.HashPin(rawPin);
            var expiresAt = DateTime.UtcNow.Add(PinLifetime);

            var handover = TransactionHandover.Create(
                transactionId: command.TransactionId,
                type: command.Type,
                pin: rawPin,
                tokenHash: pinHash,
                generatedForUserId: command.GeneratedForUserId,
                verifiedByUserId: command.VerifiedByUserId,
                expiresAt: expiresAt
            );

            await _repository.AddAsync(handover, ct);

            // QR payload encodes enough info for the scanner to call the verify endpoint
            var qrPayload = JsonSerializer.Serialize(new
            {
                handoverId = handover.Id,
                transactionId = command.TransactionId,
                type = command.Type.ToString(),
                pin = rawPin
            });

            return new GenerateHandoverResponse(
                HandoverId: handover.Id,
                Pin: rawPin,
                QrPayload: qrPayload,
                Type: command.Type,
                ExpiresAt: expiresAt
            );
        }
    }
