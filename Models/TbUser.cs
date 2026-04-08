using System;
using System.Collections.Generic;

namespace ProjectManagement.Models;

public partial class TbUser
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? PasswordHash { get; set; }

    public string? Role { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<TbAttachment> TbAttachments { get; set; } = new List<TbAttachment>();

    public virtual ICollection<TbComment> TbComments { get; set; } = new List<TbComment>();

    public virtual ICollection<TbNotification> TbNotifications { get; set; } = new List<TbNotification>();

    public virtual ICollection<TbProjectMember> TbProjectMembers { get; set; } = new List<TbProjectMember>();

    public virtual ICollection<TbProject> TbProjects { get; set; } = new List<TbProject>();

    public virtual ICollection<TbTask> TbTaskAssignedToNavigations { get; set; } = new List<TbTask>();

    public virtual ICollection<TbTask> TbTaskCreatedByNavigations { get; set; } = new List<TbTask>();

    public virtual ICollection<TbTaskHistory> TbTaskHistories { get; set; } = new List<TbTaskHistory>();
}
