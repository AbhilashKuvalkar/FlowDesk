using FlowDesk.TicketService.GraphQL.DataLoaders;
using FlowDesk.TicketService.GraphQL.Queries;
using HotChocolate.Execution.Configuration;

namespace FlowDesk.TicketService.GraphQL.Schema;

public static class GraphQLServiceExtensions
{
    public static IRequestExecutorBuilder AddFlowDeskGraphQl(this IServiceCollection services)
    {
        return services
            .AddGraphQLServer()
            .AddQueryType<TicketQuery>()
            .AddDataLoader<AgentDataLoader>()
            .AddFiltering()
            .AddSorting()
            ;
    }
}
