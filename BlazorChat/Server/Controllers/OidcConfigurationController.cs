using Microsoft.AspNetCore.Mvc;

namespace BlazorChat.Server.Controllers
{
    public class OidcConfigurationController : Controller
    {

        //public OidcConfigurationController(IClientRequestParametersProvider clientRequestParametersProvider, ILogger<OidcConfigurationController> logger)
        //{
        //    ClientRequestParametersProvider = clientRequestParametersProvider;
        //    _logger = logger;
        //}

        //public IClientRequestParametersProvider ClientRequestParametersProvider { get; }

        //[HttpGet("_configuration/{clientId}")]
        //public IActionResult GetClientRequestParameters([FromRoute] string clientId)
        //{
        //    var parameters = ClientRequestParametersProvider.GetClientParameters(HttpContext, clientId);
        //    return Ok(parameters);
        //}
    }
}
