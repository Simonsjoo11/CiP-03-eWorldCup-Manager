using EWorldCup.Api.Controllers;
using EWorldCup.Api.DTO.Responses;
using EWorldCup.Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using static EWorldCup.Api.Tests.TestUtils.CancellationTokenUtils;

namespace EWorldCup.Api.Tests.Controllers
{
    public class PlayersControllerTests
    {
        [Fact]
        public async Task GetSchedule_Should_Return_200_With_PlayerScheduleResponse()
        {
            var mockService = new Mock<ITournamentService>();
            var expected = new PlayerScheduleResponse
            {
                Player = "Charlie",
                PlayerIndex = 2,
                N = 6,
                Schedule =
                [
                    new PlayerScheduleItemDto { Round = 1, Opponent = "Ethan",  OpponentIndex = 4 },
                new PlayerScheduleItemDto { Round = 2, Opponent = "Alice",  OpponentIndex = 0 },
            ]
            };

            mockService.Setup(s => s.GetPlayerScheduleAsync(2, None)).ReturnsAsync(expected);
            var controller = new PlayersController(mockService.Object);

            var result = await controller.GetSchedule(2, None);

            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetSchedule_Should_Return_400_When_ArgumentException()
        {
            var mockService = new Mock<ITournamentService>();
            mockService.Setup(s => s.GetPlayerScheduleAsync(It.IsAny<int>(), None))
                       .ThrowsAsync(new ArgumentException("Invalid index"));

            var controller = new PlayersController(mockService.Object);

            var result = await controller.GetSchedule(-1, None);

            var bad = result as BadRequestObjectResult;
            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(400);
            bad.Value.Should().BeEquivalentTo(new { ok = false, message = "Invalid index" });
        }

        [Fact]
        public async Task GetPlayerInRound_Should_Return_200_With_PlayerRoundResponse()
        {
            var mockService = new Mock<ITournamentService>();
            var expected = new PlayerRoundResponse
            {
                Round = 3,
                PlayerIndex = 2,
                Player = "Charlie",
                OpponentIndex = 5,
                Opponent = "Fiona"
            };

            mockService.Setup(s => s.GetPlayerInRoundAsync(2, 3, None)).ReturnsAsync(expected);
            var controller = new PlayersController(mockService.Object);

            var result = await controller.GetPlayerInRound(2, 3, None);

            var ok = result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetPlayerInRound_Should_Return_400_On_ArgumentException()
        {
            var mockService = new Mock<ITournamentService>();
            mockService.Setup(s => s.GetPlayerInRoundAsync(It.IsAny<int>(), It.IsAny<int>(), None))
                       .ThrowsAsync(new ArgumentException("Out of range"));

            var controller = new PlayersController(mockService.Object);

            var result = await controller.GetPlayerInRound(999, 999, None);

            var bad = result as BadRequestObjectResult;
            bad.Should().NotBeNull();
            bad!.StatusCode.Should().Be(400);
            bad.Value.Should().BeEquivalentTo(new { message = "Out of range" });
        }

        [Fact]
        public async Task GetPlayerInRound_Should_Return_500_On_InvalidOperationException()
        {
            var mockService = new Mock<ITournamentService>();
            mockService.Setup(s => s.GetPlayerInRoundAsync(It.IsAny<int>(), It.IsAny<int>(), None))
                       .ThrowsAsync(new InvalidOperationException("Internal scheduling error"));

            var controller = new PlayersController(mockService.Object);

            var result = await controller.GetPlayerInRound(1, 1, None);

            var obj = result as ObjectResult;
            obj.Should().NotBeNull();
            obj!.StatusCode.Should().Be(500);
            obj.Value.Should().BeEquivalentTo(new { message = "Internal scheduling error" });
        }
    }
}