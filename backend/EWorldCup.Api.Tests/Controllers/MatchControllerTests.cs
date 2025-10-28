using System;
using System.Threading.Tasks;
using EWorldCup.Api.Controllers;
using EWorldCup.Api.DTO.Responses;
using EWorldCup.Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using static EWorldCup.Api.Tests.TestUtils.CancellationTokenUtils;

namespace EWorldCup.Api.Tests.Controllers
{
    public class MatchControllerTests
    {
        [Fact]
        public async Task GetDirectMatch_Should_Return_200_With_PlayerRoundResponse()
        {
            var mockService = new Mock<ITournamentService>();
            var expected = new PlayerRoundResponse
            {
                Round = 5,
                PlayerIndex = 2,
                Player = "Charlie",
                OpponentIndex = 6,
                Opponent = "George"
            };

            mockService.Setup(s => s.GetPlayerInRoundAsync(2, 5, None)).ReturnsAsync(expected);
            var controller = new MatchController(mockService.Object);

            var result = await controller.GetDirectMatch(2, 5, None);

            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetDirectMatch_Should_Return_400_On_ArgumentException()
        {
            var mockService = new Mock<ITournamentService>();
            mockService.Setup(s => s.GetPlayerInRoundAsync(It.IsAny<int>(), It.IsAny<int>(), None))
                       .ThrowsAsync(new ArgumentException("Out of range"));

            var controller = new MatchController(mockService.Object);

            var result = await controller.GetDirectMatch(-1, 999, None);

            var bad = result as BadRequestObjectResult;
            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(400);
            bad.Value.Should().BeEquivalentTo(new { message = "Out of range" });
        }

        [Fact]
        public async Task GetDirectMatch_Should_Return_500_On_InvalidOperationException()
        {
            var mockService = new Mock<ITournamentService>();
            mockService.Setup(s => s.GetPlayerInRoundAsync(It.IsAny<int>(), It.IsAny<int>(), None))
                       .ThrowsAsync(new InvalidOperationException("Internal scheduling error"));

            var controller = new MatchController(mockService.Object);

            var result = await controller.GetDirectMatch(1, 1, None);

            var obj = result as ObjectResult;
            obj.Should().NotBeNull();
            obj!.StatusCode.Should().Be(500);
            obj.Value.Should().BeEquivalentTo(new { message = "Internal scheduling error" });
        }

        [Fact]
        public async Task GetRemainingPairs_Should_Return_200_With_RemainingPairsResponse()
        {
            var mockService = new Mock<ITournamentService>();
            var expected = new RemainingPairsResponse
            {
                ParticipantCount = 8,
                RoundsPlayed = 3,
                Remaining = 9,
                TotalPairs = 28
            };

            mockService.Setup(s => s.GetRemainingPairsAsync(8, 3, None)).ReturnsAsync(expected);
            var controller = new MatchController(mockService.Object);

            var result = await controller.GetRemainingPairs(8, 3, None);

            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetRemainingPairs_Should_Return_400_On_ArgumentException()
        {
            var mockService = new Mock<ITournamentService>();
            mockService.Setup(s => s.GetRemainingPairsAsync(It.IsAny<int?>(), It.IsAny<int?>(), None))
                       .ThrowsAsync(new ArgumentException("n must be even."));

            var controller = new MatchController(mockService.Object);

            var result = await controller.GetRemainingPairs(3, 1, None);

            var bad = result as BadRequestObjectResult;
            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(400);
            bad.Value.Should().BeEquivalentTo(new { message = "n must be even." });
        }
    }
}
