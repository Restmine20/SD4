using Microsoft.AspNetCore.Mvc;

namespace ApiGateway.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GatewayController : ControllerBase
{
  private readonly HttpClient _orderServiceClient;
  private readonly HttpClient _paymentServiceClient;


  public GatewayController(IHttpClientFactory httpClientFactory)
  {
    _orderServiceClient = httpClientFactory.CreateClient("OrderService");
    _paymentServiceClient = httpClientFactory.CreateClient("PaymentService");
  }


  [HttpPost("orders")]
  public async Task<IActionResult> CreateOrder([FromForm] Guid userId, [FromForm] string? description, [FromForm] decimal totalAmount)
  {
    var formData = new Dictionary<string, string?>
    {
      ["userId"] = userId.ToString(),
      ["description"] = description,
      ["totalAmount"] = totalAmount.ToString(System.Globalization.CultureInfo.InvariantCulture)
    };

    using var requestContent = new FormUrlEncodedContent(formData);

    using var response = await _orderServiceClient.PostAsync("/orders", requestContent);

    var responseBody = await response.Content.ReadAsStringAsync();
    return new ContentResult
    {
      Content = responseBody,
      ContentType = response.Content.Headers.ContentType?.ToString(),
      StatusCode = (int)response.StatusCode
    };
  }

  [HttpGet("orders/user/{user_id}")]
  public IActionResult GetOrders(Guid user_id)
  {
    using var response = _orderServiceClient.GetAsync($"/orders/user/{user_id}").Result;

    var responseBody = response.Content.ReadAsStringAsync().Result;

    return new ContentResult
    {
      Content = responseBody,
      ContentType = response.Content.Headers.ContentType?.ToString(),
      StatusCode = (int)response.StatusCode
    };
  }

  [HttpGet("orders/{order_id}")]
  public IActionResult GetOrder(Guid order_id)
  {
    using var response = _orderServiceClient.GetAsync($"/orders/{order_id}").Result;

    var responseBody = response.Content.ReadAsStringAsync().Result;

    return new ContentResult
    {
      Content = responseBody,
      ContentType = response.Content.Headers.ContentType?.ToString(),
      StatusCode = (int)response.StatusCode
    };
  }





  [HttpPost("accounts")]
  public IActionResult CreateAccount([FromForm] Guid userId)
  {
    var formData = new Dictionary<string, string?>
    {
      ["userId"] = userId.ToString()
    };

    using var requestContent = new FormUrlEncodedContent(formData);

    using var response = _paymentServiceClient.PostAsync("/accounts", requestContent).Result;

    var responseBody = response.Content.ReadAsStringAsync().Result;

    return new ContentResult
    {
      Content = responseBody,
      ContentType = response.Content.Headers.ContentType?.ToString(),
      StatusCode = (int)response.StatusCode
    };
  }


  [HttpPost("accounts/deposit")]
  public IActionResult Deposit([FromForm] Guid id, [FromForm] decimal amount)
  {
    var formData = new Dictionary<string, string?>
    {
      ["id"] = id.ToString(),
      ["amount"] = amount.ToString(System.Globalization.CultureInfo.InvariantCulture)
    };

    using var requestContent = new FormUrlEncodedContent(formData);

    using var response = _paymentServiceClient.PostAsync("/accounts/deposit", requestContent).Result;

    var responseBody = response.Content.ReadAsStringAsync().Result;
    return new ContentResult
    {
      Content = responseBody,
      ContentType = response.Content.Headers.ContentType?.ToString(),
      StatusCode = (int)response.StatusCode
    };
  }


  [HttpGet("accounts/{id}")]
  public IActionResult GetBalance(Guid id)
  {
    using var response = _paymentServiceClient.GetAsync($"/accounts/{id}").Result;

    var responseBody = response.Content.ReadAsStringAsync().Result;
    return new ContentResult
    {
      Content = responseBody,
      ContentType = response.Content.Headers.ContentType?.ToString(),
      StatusCode = (int)response.StatusCode
    };
  }
}