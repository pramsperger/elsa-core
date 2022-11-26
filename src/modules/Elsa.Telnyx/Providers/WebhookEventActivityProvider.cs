using System.ComponentModel;
using System.Reflection;
using Elsa.Telnyx.Activities;
using Elsa.Telnyx.Attributes;
using Elsa.Telnyx.Helpers;
using Elsa.Telnyx.Payloads.Abstract;
using Elsa.Workflows.Core.Models;
using Elsa.Workflows.Management.Extensions;
using Elsa.Workflows.Management.Services;

namespace Elsa.Telnyx.Providers;

/// <summary>
/// Provides activity descriptors based on Telnyx webhook event payload types (types inheriting <see cref="Payload"/>. 
/// </summary>
public class WebhookEventActivityProvider : IActivityProvider
{
    private readonly IActivityFactory _activityFactory;

    /// <summary>
    /// Constructor.
    /// </summary>
    public WebhookEventActivityProvider(IActivityFactory activityFactory)
    {
        _activityFactory = activityFactory;
    }


    /// <inheritdoc />
    public ValueTask<IEnumerable<ActivityDescriptor>> GetDescriptorsAsync(CancellationToken cancellationToken = default)
    {
        var payloadTypes = WebhookPayloadTypes.PayloadTypes;
        var descriptors = CreateDescriptors(payloadTypes).ToList();
        return new(descriptors);
    }

    private IEnumerable<ActivityDescriptor> CreateDescriptors(IEnumerable<Type> jobTypes) => jobTypes.Select(CreateDescriptor);

    private ActivityDescriptor CreateDescriptor(Type payloadType)
    {
        var webhookAttribute = payloadType.GetCustomAttribute<WebhookAttribute>() ?? throw new Exception($"No WebhookAttribute found on payload type {payloadType}");
        var ns = Constants.Namespace;
        var typeName = webhookAttribute.ActivityType;
        var fullTypeName = $"{ns}.{typeName}";
        var displayNameAttr = payloadType.GetCustomAttribute<DisplayNameAttribute>();
        var displayName = displayNameAttr?.DisplayName ?? webhookAttribute.DisplayName;
        var categoryAttr = payloadType.GetCustomAttribute<CategoryAttribute>();
        var category = categoryAttr?.Category ?? Constants.Category;
        var descriptionAttr = payloadType.GetCustomAttribute<DescriptionAttribute>();
        var description = descriptionAttr?.Description ?? webhookAttribute?.Description;

        return new()
        {
            TypeName = fullTypeName,
            Version = 1,
            DisplayName = displayName,
            Description = description,
            Category = category,
            Kind = ActivityKind.Job,
            IsBrowsable = true,
            ActivityType = typeof(WebhookEvent),
            Constructor = context =>
            {
                var activity = _activityFactory.Create<WebhookEvent>(context);
                activity.Type = fullTypeName;
                activity.EventType = new Input<string>(webhookAttribute!.EventType);

                return activity;
            }
        };
    }
}