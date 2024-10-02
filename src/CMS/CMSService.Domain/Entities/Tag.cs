using Weblog.Shared.Entities;

namespace CMSService.Domain.Entities;

public class Tag : BaseEntity<Guid>
{
    public string Name { get; set; }
}