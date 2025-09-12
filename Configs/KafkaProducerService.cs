using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using ECS_Logistics.DTOs;
using ECS_Logistics.Utils;

namespace ECS_Logistics.Configs;

public class KafkaProducerService
{
    private readonly IProducer<string, string> _producer;

    public KafkaProducerService()
    {
        var config = new ProducerConfig
        {
            BootstrapServers = "localhost:9092"
        };
        _producer = new ProducerBuilder<string, string>(config).Build();
    }

    public async Task SendOrderTrackingUpdateAsync(OrderTrackingEnrichedDto dto)
    {
        var message = new Message<string, string>
        {
            Key = dto.OrderItem?.OrderItemId.ToString() ?? string.Empty,
            Value = JsonSerializer.Serialize(dto, HelperFunctions.JsonSerializerOptions)
        };

        await _producer.ProduceAsync("order-tracking-updates", message);
    }
}

public class JsonDateTimeConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.Parse(reader.GetString()!);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("yyyy-MM-dd HH:mm:ss"));
    }
}