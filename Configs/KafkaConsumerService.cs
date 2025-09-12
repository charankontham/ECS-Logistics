using System.Text.Json;
using Confluent.Kafka;
using ECS_Logistics.DTOs;
using ECS_Logistics.Services;
using ECS_Logistics.Utils;
using static System.Console;

namespace ECS_Logistics.Configs;

public class KafkaConsumerService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IConsumer<string, string> _consumer;
    private const string TopicName = "order-tracking-create";
    private readonly KafkaProducerService _kafkaProducerService;

    public KafkaConsumerService(IServiceProvider serviceProvider, KafkaProducerService kafkaProducerService)
    {
        _serviceProvider = serviceProvider;
        _kafkaProducerService = kafkaProducerService;
        var config = new ConsumerConfig
        {
            BootstrapServers = "localhost:9092",
            GroupId = "ecs-logistics-consumer-group",
            GroupInstanceId = "ecs-logistics-consumer-group-1",
            AutoOffsetReset = AutoOffsetReset.Earliest,
            SocketTimeoutMs = 10000,
            SessionTimeoutMs = 6000,
            EnableAutoCommit = false
        };
        _consumer = new ConsumerBuilder<string, string>(config).Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _consumer.Subscribe(TopicName);
            WriteLine("KafkaConsumerService subscribed...");
            while (!stoppingToken.IsCancellationRequested)
            {
                WriteLine("KafkaConsumerService inside while loop");
                try
                {
                    WriteLine("Printing the stopping token : ", stoppingToken.ToString());
                    var consumeResult = await Task.Run(() => _consumer.Consume(stoppingToken), stoppingToken);
                    WriteLine("KafkaConsumerService after consuming the message");
                    if(consumeResult?.Message == null) continue;
                    var message = consumeResult.Message.Value;
                    try
                    {
                        var trackingList = JsonSerializer.Deserialize<List<OrderTrackingDto>>
                            (message, HelperFunctions.JsonSerializerOptions);
                        WriteLine("KafkaConsumerService after deserializing the order tracking list");
                        if (trackingList == null) continue;
                        using var scope = _serviceProvider.CreateScope();
                        var trackingService = scope.ServiceProvider.GetRequiredService<IOrderTrackingService>();
                        WriteLine("KafkaConsumerService after fetching the order tracking service");
                        foreach (var orderTrackingDto in trackingList)
                        {
                            try
                            {
                                var response = await trackingService.CreateAsync(orderTrackingDto);
                                if (response is OrderTrackingEnrichedDto responseDto)
                                {
                                    await _kafkaProducerService.SendOrderTrackingUpdateAsync(responseDto);
                                    _consumer.Commit(consumeResult);
                                    WriteLine("KafkaConsumerService Published status to order-tracking-updates");
                                }
                                else
                                {
                                    WriteLine("KafkaConsumerService couldn't create order tracking response");
                                }
                            }
                            catch (Exception e)
                            {
                                WriteLine($"Failed to create OrderTracking/send orderTrackingUpdates : {e}");
                            }
                        }
                    }
                    catch (JsonException jsonEx)
                    {
                        WriteLine($"Deserialization error: {jsonEx.Message}");
                    }
                }
                catch (ConsumeException ce)
                {
                    WriteLine($"Kafka consume error: {ce.Error.Reason}");
                }
                catch (Exception ex)
                {
                    WriteLine($"Error in Kafka consumer: {ex}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            WriteLine("Kafka consumer stopped");
        }
        catch (Exception ex)
        {
            WriteLine($"Failed to subscribe to topic: {ex}");
        }
        finally
        {
            WriteLine("Closing KafkaConsumer...");
            _consumer.Close();
        }
    }
    
    public override void Dispose()
    {
        _consumer.Dispose();
        base.Dispose();
    }
}