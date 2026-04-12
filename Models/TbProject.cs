using System;
using System.Collections.Generic;

namespace ProjectManagement.Models;

public partial class TbProject
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Description { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual TbUser? CreatedByNavigation { get; set; }

    public virtual ICollection<TbProjectMember> TbProjectMembers { get; set; } = new List<TbProjectMember>();

    public virtual ICollection<TbTask> TbTasks { get; set; } = new List<TbTask>();
}
