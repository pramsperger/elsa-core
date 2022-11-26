using System.ComponentModel;
using System.Reflection;
using Elsa.Abstractions;
using Elsa.Workflows.Core;
using Elsa.Workflows.Core.Services;
using Elsa.Workflows.Management.Models;
using Elsa.Workflows.Management.Options;
using Microsoft.Extensions.Options;

namespace Elsa.Workflows.Api.Endpoints.VariableTypes.List;

/// <summary>
/// Returns a lis tof available variable types.
/// </summary>
public class List : ElsaEndpointWithoutRequest<Response>
{
    private readonly IWellKnownTypeRegistry _wellKnownTypeRegistry;
    private readonly ManagementOptions _managementOptions;

    /// <inheritdoc />
    public List(IOptions<ManagementOptions> managementOptions, IWellKnownTypeRegistry wellKnownTypeRegistry)
    {
        _wellKnownTypeRegistry = wellKnownTypeRegistry;
        _managementOptions = managementOptions.Value;
    }

    /// <inheritdoc />
    public override void Configure()
    {
        Get("/descriptors/variables");
        ConfigurePermissions("read:*", "read:variable-descriptors");
    }

    /// <inheritdoc />
    public override Task<Response> ExecuteAsync(CancellationToken cancellationToken)
    {
        var types = _managementOptions.VariableDescriptors;
        var descriptors = types.Select(Map).ToList();
        var response = new Response(descriptors);

        return Task.FromResult(response);
    }

    private VariableDescriptorModel Map(VariableDescriptor descriptor)
    {
        var type = descriptor.Type;
        var hasAlias = _wellKnownTypeRegistry.TryGetAlias(type, out var alias);
        var typeName = hasAlias ? alias : type.GetSimpleAssemblyQualifiedName()!;
        var displayName = hasAlias ? alias : type.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? type.Name;
        var description = descriptor.Description ?? type.GetCustomAttribute<DescriptionAttribute>()?.Description;

        return new(typeName, displayName, descriptor.Category, description);
    }
}