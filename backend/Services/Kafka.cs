using System.Threading.Tasks;

namespace Backend.Services.Kafka
{
    public interface IKafkaProducer
    {
        Task PublishAsync(string topic, string key, string value, CancellationToken cancellationToken = default);
    }
}


