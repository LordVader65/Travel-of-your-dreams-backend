using RabbitMQ.Client;

namespace TravelDreams.ApiGateway.Marketplace.V3;

internal static class RabbitMqConnectionFactoryV3
{
    public static ConnectionFactory Create(RabbitMqOptionsV3 options)
    {
        var factory = new ConnectionFactory
        {
            DispatchConsumersAsync = true,
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true,
            NetworkRecoveryInterval = TimeSpan.FromSeconds(10),
            RequestedHeartbeat = TimeSpan.FromSeconds(30)
        };

        if (!string.IsNullOrWhiteSpace(options.Uri))
        {
            factory.Uri = new Uri(options.Uri, UriKind.Absolute);
            return factory;
        }

        factory.HostName = options.Host;
        factory.Port = options.Port;
        factory.UserName = options.User;
        factory.Password = options.Password;
        factory.VirtualHost = string.IsNullOrWhiteSpace(options.VirtualHost) ? "/" : options.VirtualHost;

        if (options.UseTls)
        {
            factory.Ssl.Enabled = true;
            factory.Ssl.ServerName = options.Host;
        }

        return factory;
    }
}
