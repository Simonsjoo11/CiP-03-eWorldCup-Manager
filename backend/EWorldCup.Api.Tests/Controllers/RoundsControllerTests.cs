using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EWorldCup.Api.Controllers;
using EWorldCup.Api.DTO.Responses;
using EWorldCup.Api.Models;
using EWorldCup.Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using static EWorldCup.Api.Tests.TestUtils.CancellationTokenUtils;

namespace EWorldCup.Api.Tests.Controllers
{
    public class RoundsControllerTests
    {
        [Fact]
        public void GetMaxRounds_Should_Return_400_If_n_Not_Integer()
        {
            var svc = new Mock<ITournamentService>();
            var controller = new RoundsController(svc.Object);

            var result = controller.GetMaxRounds("abc");

            var bad = result as BadRequestObjectResult;
            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(400);
            bad.Value.Should().BeEquivalentTo(new { ok = false, message = "n must be an integer." });
        }

        [Theory]
        [InlineData("1", "n must be ≥ 2.")]
        [InlineData("3", "n must be even.")]
        public void GetMaxRounds_Should_Validate_n_Rules(string n, string expectedMessage)
        {
            var svc = new Mock<ITournamentService>();
            var controller = new RoundsController(svc.Object);

            var result = controller.GetMaxRounds(n);

            var bad = result as BadRequestObjectResult;
            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(400);
            bad.Value.Should().BeEquivalentTo(new { ok = false, message = expectedMessage });
        }

        [Fact]
        public void GetMaxRounds_Should_Return_200_With_BigInteger_Strings_When_n_Ok()
        {
            var svc = new Mock<ITournamentService>();
            var controller = new RoundsController(svc.Object);

            var result = controller.GetMaxRounds("10");

            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(new { ok = true, n = "10", max = "9" });
        }

        [Fact]
        public void GetMaxRounds_Should_Use_Service_When_n_Not_Provided()
        {
            var svc = new Mock<ITournamentService>();
            svc.Setup(s => s.GetMaxRounds(null)).Returns(7);
            var controller = new RoundsController(svc.Object);

            var result = controller.GetMaxRounds(null);

            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(new { ok = true, max = 7 });
        }

        [Fact]
        public void GetMaxRounds_Should_Return_400_When_Service_Throws_ArgumentException()
        {
            var svc = new Mock<ITournamentService>();
            svc.Setup(s => s.GetMaxRounds(null)).Throws(new ArgumentException("No participants"));
            var controller = new RoundsController(svc.Object);

            var result = controller.GetMaxRounds(null);

            var bad = result as BadRequestObjectResult;
            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(400);
            bad.Value.Should().BeEquivalentTo(new { ok = false, message = "No participants" });
        }

        [Fact]
        public async Task GetRound_Should_Return_200_With_RoundResponse()
        {
            var svc = new Mock<ITournamentService>();
            var expected = new RoundResponse
            {
                Round = 2,
                Pairs =
                [
                    new MatchPair("Alice", "Charlie"),
                new MatchPair("Bob", "Fiona"),
            ]
            };
            svc.Setup(s => s.GetRoundAsync(2, None)).ReturnsAsync(expected);

            var controller = new RoundsController(svc.Object);

            var result = await controller.GetRound(2, None);

            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetRound_Should_Return_400_When_ArgumentException()
        {
            var svc = new Mock<ITournamentService>();
            svc.Setup(s => s.GetRoundAsync(It.IsAny<int>(), None))
               .ThrowsAsync(new ArgumentException("Round out of range"));

            var controller = new RoundsController(svc.Object);

            var result = await controller.GetRound(999, None);

            var bad = result as BadRequestObjectResult;
            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(400);
            bad.Value.Should().BeEquivalentTo(new { message = "Round out of range" });
        }
    }
}
