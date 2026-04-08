using System;
using System.Collections.Generic;

namespace ProjectManagement.Models;

public partial class TbAttachment
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public string? FileUrl { get; set; }

    public int? UploadedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual TbTask? Task { get; set; }

    public virtual TbUser? UploadedByNavigation { get; set; }
}
