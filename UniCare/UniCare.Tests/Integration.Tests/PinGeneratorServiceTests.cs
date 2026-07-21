using System.Linq;
using UniCare.Infrastructure.Repositories;
using Xunit;

namespace UniCare.Tests.Services
{
    
    public class PinGeneratorServiceTests
    {
        private readonly PinGeneratorService _service = new();

        [Fact]
        public void GeneratePin_ReturnsSixDigitNumericString()
        {
            var pin = _service.GeneratePin();

            Assert.Equal(6, pin.Length);
            Assert.True(pin.All(char.IsDigit));
        }

        [Fact]
        public void GeneratePin_CalledMultipleTimes_ProducesVariedValues()
        {
            var pins = Enumerable.Range(0, 20).Select(_ => _service.GeneratePin()).ToList();

            // Extremely unlikely all 20 six-digit PINs are identical if generation is random.
            Assert.True(pins.Distinct().Count() > 1);
        }

        [Fact]
        public void HashPin_SamePinTwice_ProducesSameHash()
        {
            var hash1 = _service.HashPin("123456");
            var hash2 = _service.HashPin("123456");

            Assert.Equal(hash1, hash2);
        }

        [Fact]
        public void HashPin_DifferentPins_ProduceDifferentHashes()
        {
            var hash1 = _service.HashPin("123456");
            var hash2 = _service.HashPin("654321");

            Assert.NotEqual(hash1, hash2);
        }

        [Fact]
        public void VerifyPin_CorrectPin_ReturnsTrue()
        {
            const string pin = "654321";
            var hash = _service.HashPin(pin);

            Assert.True(_service.VerifyPin(pin, hash));
        }

        [Fact]
        public void VerifyPin_IncorrectPin_ReturnsFalse()
        {
            var hash = _service.HashPin("111111");

            Assert.False(_service.VerifyPin("222222", hash));
        }
    }
}
