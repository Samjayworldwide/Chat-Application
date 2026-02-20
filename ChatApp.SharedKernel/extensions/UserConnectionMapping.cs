using System.Collections.Concurrent;

namespace ChatApp.SharedKernel.extensions;

public static class UserConnectionMapping
{
    private static readonly ConcurrentDictionary<string, HashSet<string>> Connections = new();

    public static void AddConnection(string userId, string connectionId)
    {
        var connections = Connections.GetOrAdd(userId, _ => []);

        lock (connections)
        {
            connections.Add(connectionId);
        }
    }

    public static void RemoveConnection(string userId, string connectionId)
    {
        if (!Connections.TryGetValue(userId, out var connections))
            return;

        lock (connections)
        {
            connections.Remove(connectionId);

            if (connections.Count == 0)
            {
                Connections.TryRemove(userId, out _);
            }
        }
    }

    public static IEnumerable<string> GetConnections(string userId)
    {
        if (!Connections.TryGetValue(userId, out var connections))
            return [];

        lock (connections)
        {
            return connections.ToList();
        }
    }
}