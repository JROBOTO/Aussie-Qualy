namespace JR.AussieQualy.Tests.Integration;

using System.Net;
using System.Net.Http.Json;
using JR.AussieQualy.Application.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using NUnit.Framework;

public sealed class QualifyingEndpointsTests
{
    private WebApplicationFactory<JR.AussieQualy.Api.ProgramMarker> _factory = null!;
    private HttpClient _client = null!;

    [SetUp]
    public void SetUp()
    {
        _factory = new WebApplicationFactory<JR.AussieQualy.Api.ProgramMarker>();
        _client = _factory.CreateClient();
    }

    [TearDown]
    public void TearDown()
    {
        _client.Dispose();
        _factory.Dispose();
    }

    [Test]
    public async Task QualifyingEndpoint_ReturnsNotFound_WhenCodeMissing()
    {
        HttpResponseMessage response = await _client.GetAsync("/qualifying/ ");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task QualifyingEndpoint_ReturnsNotFound_WhenDriverDoesNotExist()
    {
        HttpResponseMessage response = await _client.GetAsync("/qualifying/XXX");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.NotFound));
    }

    [Test]
    public async Task QualifyingEndpoint_ReturnsData_WhenDriverExists()
    {
        HttpResponseMessage response = await _client.GetAsync("/qualifying/HAM");

        Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK));

        DriverQualifyingResultDto? dto =
            await response.Content.ReadFromJsonAsync<DriverQualifyingResultDto>();

        Assert.That(dto, Is.Not.Null);
        Assert.That(dto!.Driver.Name, Is.Not.Empty);
    }
}
