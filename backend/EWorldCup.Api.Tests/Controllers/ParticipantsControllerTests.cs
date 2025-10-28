using EWorldCup.Api.Controllers;
using EWorldCup.Api.DTO.Responses;
using EWorldCup.Api.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using static EWorldCup.Api.Tests.TestUtils.CancellationTokenUtils;

namespace EWorldCup.Api.Tests.Controllers
{
    public class ParticipantsControllerTests
    {
        [Fact]
        public async Task GetAll_Should_Return_200_With_ParticipantsResponse()
        {
            var mockService = new Mock<ITournamentService>();
            var expected = new ParticipantsResponse
            {
                Participants =
                [
                    new ParticipantDto { Id = 1, Name = "Alice" },
                new ParticipantDto { Id = 2, Name = "Bob" }
                ]
            };

            mockService.Setup(s => s.GetParticipantsAsync(None)).ReturnsAsync(expected);
            var controller = new ParticipantsController(mockService.Object);

            var result = await controller.GetAll(None);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task GetAll_Should_Return_200_With_Empty_List_When_No_Participants()
        {
            var mockService = new Mock<ITournamentService>();
            var expected = new ParticipantsResponse { Participants = [] };

            mockService.Setup(s => s.GetParticipantsAsync(None)).ReturnsAsync(expected);
            var controller = new ParticipantsController(mockService.Object);

            var result = await controller.GetAll(None);

            var ok = result.Result as OkObjectResult;
            ok.Should().NotBeNull();
            ok!.StatusCode.Should().Be(200);
            ok.Value.Should().BeEquivalentTo(expected);
        }
    }
}
