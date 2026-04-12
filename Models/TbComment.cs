using System;
using System.Collections.Generic;

namespace ProjectManagement.Models;

public partial class TbComment
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public int? UserId { get; set; }

    public string? Content { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual TbTask? Task { get; set; }

    public virtual TbUser? User { get; set; }
}
