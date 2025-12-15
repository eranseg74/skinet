using System.Collections.Concurrent;
using API.Extensions;
using Microsoft.AspNetCore.SignalR;

namespace API.SignalR;

// We are using SignalR to notify the client on a successful payment (or not) in Stripe. This is because stripe notifies the server about the success but the client does not know anything about that. The server will update the client on the success using SignalR.
// In order to use SignalR we just need to inherit from Hub. The idea is to notify the client by the user email address.
// SignalR does not manage connections by user email but only keeps track on the client connection id and that is what the browser uses to maintain a connection with the SignalR service. To control which email is connected to the client connection we will need to store it in the hub. The implementation here is for simplicity but we might want to keep it in a DB such as Redis
public class NotificationHub : Hub
{
  private static readonly ConcurrentDictionary<string, string> UserConnections = new();

  public override Task OnConnectedAsync()
  {
    var email = Context.User?.GetEmail();
    if (!string.IsNullOrEmpty(email)) UserConnections[email] = Context.ConnectionId;
    return base.OnConnectedAsync();
  }

  public override Task OnDisconnectedAsync(Exception? exception)
  {
    var email = Context.User?.GetEmail();
    if (!string.IsNullOrEmpty(email)) UserConnections.TryRemove(email, out _);
    return base.OnDisconnectedAsync(exception);
  }

  public static string? GetConnectionIdByEmail(string email)
  {
    UserConnections.TryGetValue(email, out var connectionId);
    return connectionId;
  }
}
