using System;
using System.Collections.Generic;

namespace CollectionGallery.Shared;

public sealed class Constants
{
    public static readonly string StorageBucket = Environment.GetEnvironmentVariable("STORAGE_BUCKET") ?? throw new KeyNotFoundException("STORAGE_BUCKET key not found");
    public static readonly string TopicName = Environment.GetEnvironmentVariable("TOPIC_NAME") ?? throw new KeyNotFoundException("TOPIC_NAME key not found");
    public static readonly string SubscriptionName = Environment.GetEnvironmentVariable("SUBSCRIPTION_NAME") ?? throw new KeyNotFoundException("SUBSCRIPTION_NAME key not found");
    public static readonly string ProjectId = Environment.GetEnvironmentVariable("PROJECT_ID") ?? throw new KeyNotFoundException("PROJECT_ID key not found");
}