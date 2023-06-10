using Elsa.Api.Client.Contracts;
using Elsa.Api.Client.Resources.ActivityDescriptors.Contracts;
using Elsa.Api.Client.Resources.WorkflowDefinitions.Contracts;
using Elsa.Api.Client.Resources.WorkflowInstances.Contracts;

namespace Elsa.Api.Client.Services;

/// <inheritdoc />
public class ElsaClient : IElsaClient
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ElsaClient"/> class.
    /// </summary>
    public ElsaClient(IWorkflowDefinitionsApi workflowDefinitions, IWorkflowInstancesApi workflowInstances, IActivityDescriptorsApi activityDescriptors)
    {
        WorkflowDefinitions = workflowDefinitions;
        WorkflowInstances = workflowInstances;
        ActivityDescriptors = activityDescriptors;
    }

    /// <inheritdoc />
    public IWorkflowDefinitionsApi WorkflowDefinitions { get; }

    /// <inheritdoc />
    public IActivityDescriptorsApi ActivityDescriptors { get; }

    /// <inheritdoc />
    public IWorkflowInstancesApi WorkflowInstances { get; }
}