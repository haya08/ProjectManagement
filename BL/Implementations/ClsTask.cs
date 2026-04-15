using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.TaskHistory;
using ProjectManagement.DTOs.Tasks;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;
using System.Security.Claims;

namespace ProjectManagement.BL.Implementations
{
    public class ClsTask : ITask
    {
        private readonly ITaskRepository _repo;
        private readonly ITaskHistory _history;
        private readonly IProjectMemberRepository _projectMemberRepo;
        private readonly IHttpContextAccessor _httpContext;

        public ClsTask(ITaskRepository repo, ITaskHistory history, IHttpContextAccessor httpContext, IProjectMemberRepository projectMemberRepo)
        {
            _repo = repo;
            _history = history;
            _httpContext = httpContext;
            _projectMemberRepo = projectMemberRepo;
        }

        public ApiResponse GetAllTasks(TaskQueryDTO query)
        {
            var result = new ApiResponse();
            try
            {
                var tasks = _repo.GetAll(query);

                result.Data = tasks.Select(t => new TasksDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    Priority = t.Priority,
                    AssignedUserName = t.AssignedToNavigation?.UserName,
                    DueDate = t.DueDate
                }).ToList(); ;

                result.StatusCode = "200";

                return result;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Errors.Add(new { Exception = ex.Message });
                result.StatusCode = "500";
                return result;
            }

        }

        public ApiResponse GetTaskById(int id)
        {
            var result = new ApiResponse();
            try
            {
                var task = _repo.GetById(id);

                if (task == null)
                {
                    result.Data = null;
                    result.Errors.Add(new { Message = "Task not found" });
                    result.StatusCode = "404";
                    return result;
                }

                result.Data = new TasksDTO
                {
                    Id = task.Id,
                    Title = task.Title,
                    Status = task.Status,
                    Priority = task.Priority,
                    AssignedUserName = task.AssignedToNavigation?.UserName,
                    DueDate = task.DueDate
                };

                result.StatusCode = "200";
                return result;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Errors.Add(new { Exception = ex.Message });
                result.StatusCode = "500";
                return result;
            }

        }

        public List<TasksDTO> GetTasksByProjectId(int id)
        {
            try
            {
                var tasks = _repo.GetByProjectId(id);

                return tasks.Select(t => new TasksDTO
                {
                    Id = t.Id,
                    Title = t.Title,
                    Status = t.Status,
                    Priority = t.Priority,
                    AssignedUserName = t.AssignedToNavigation?.UserName,
                    DueDate = t.DueDate
                }).ToList();
            }
            catch (Exception ex)
            {
                return new List<TasksDTO>();
            }
        }

        public ApiResponse CreateTask(CreateTaskDTO dto)
        {
            var result = new ApiResponse();
            try
            {
                // validations


                // Title required
                if (string.IsNullOrWhiteSpace(dto.Title))
                {
                    result.Errors.Add(new
                    {
                        Field = "Title",
                        Message = "Title is required"
                    });
                }

                // DueDate must be in the future
                if (dto.DueDate < DateTime.UtcNow)
                {
                    result.Errors.Add(new
                    {
                        Field = "DueDate",
                        Message = "Due date must be in the future"
                    });
                }

                // Priority check
                var allowedPriorities = new[] { "low", "medium", "high" };
                if (string.IsNullOrWhiteSpace(dto.Priority))
                {
                    result.Errors.Add(new
                    {
                        Field = "Priority",
                        Message = "Priority is required. Allowed values: low, medium, high"
                    });
                }
                else if (!allowedPriorities.Contains(dto.Priority.ToLower()))
                {
                    result.Errors.Add(new
                    {
                        Field = "Priority",
                        Message = "Invalid priority. Allowed values: low, medium, high"
                    });
                }

                // Project must exist
                var projectTasks = _repo.GetByProjectId(dto.ProjectId);
                if (projectTasks == null)
                {
                    result.Errors.Add(new
                    {
                        Field = "ProjectId",
                        Message = "Project not found"
                    });
                }

                if (result.Errors.Count > 0)
                {
                    result.Data = null;
                    result.StatusCode = "400";
                    return result;
                }

                // logic
                var task = new TbTask
                {
                    Title = dto.Title,
                    Description = dto.Description,
                    ProjectId = dto.ProjectId,
                    AssignedTo = dto.AssignedTo,
                    DueDate = dto.DueDate,
                    Priority = dto.Priority,
                    Status = "todo"
                };

                _repo.Add(task);
                _repo.Save();

                result.Data = new TasksDTO
                {
                    Id = task.Id,
                    Title = task.Title,
                    Status = task.Status,
                    Priority = task.Priority,
                    AssignedUserName = task.AssignedToNavigation?.UserName,
                    DueDate = task.DueDate
                };
                result.StatusCode = "201";
                return result;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Errors.Add(new { Exception = ex.Message });
                result.StatusCode = "500";
                return result;
            }
        }

        public ApiResponse UpdateTask(UpdateTaskDTO dto)
        {
            var result = new ApiResponse();
            try
            {
                // validations

                // Task must exist
                var task = _repo.GetById(dto.Id);

                if (task == null)
                {
                    result.Errors.Add(new
                    {
                        Field = "Id",
                        Message = "Task not found"
                    });
                    return result;
                }

                // if user is team member, check if they are assigned to the task
                var user = _httpContext.HttpContext.User;
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

                if (user.IsInRole("TeamMember") && task.AssignedTo != userId)
                {
                    result.Errors.Add(new
                    {
                        Field = "User",
                        Message = "You can only update tasks assigned to you"
                    });
                    result.StatusCode = "403";
                    return result;
                }


                // Title cannot be empty if provided
                if (dto.Title != null)
                {
                    if(string.IsNullOrWhiteSpace(dto.Title))
                    {
                        result.Errors.Add(new
                        {
                            Field = "Title",
                            Message = "Title cannot be empty"
                        });
                    }
                }

                // array for all history changes to be added at the end of validations
                var historyChanges = new List<CreateTaskHistoryDTO>();

                // Priority check
                var allowed = new[] { "low", "medium", "high" };
                if (dto.Priority != null && dto.Priority != task.Priority)
                {
                    if (!allowed.Contains(dto.Priority.ToLower()))
                    {
                        result.Errors.Add(new
                        {
                            Field = "Priority",
                            Message = "Invalid priority"
                        });
                    }
                    else
                    {
                        // create TaskHistoryDTO
                        var history = new CreateTaskHistoryDTO
                        {
                            TaskId = dto.Id,
                            FieldChanged = "priority",
                            OldValue = task.Priority,
                            NewValue = dto.Priority,
                            ChangedBy = task.CreatedBy
                        };

                        // add to historyChanges list
                        historyChanges.Add(history);
                    }
                    
                }

                // DueDate must be in the future
                if (dto.DueDate.HasValue)
                {
                    if (dto.DueDate < DateTime.UtcNow)
                    {
                        result.Errors.Add(new
                        {
                            Field = "DueDate",
                            Message = "Due date must be in the future"
                        });
                    }
                }

                // Status check
                if (!string.IsNullOrEmpty(dto.Status) && dto.Status != task.Status)
                {
                    var validTransitions = new Dictionary<string, string[]>
                    {
                        { "todo", new[] { "in_progress" } },
                        { "in_progress", new[] { "done" } },
                        { "done", new string[] { } }
                    };

                    var current = task.Status?.ToLower();
                    var next = dto.Status.ToLower();

                    if (!validTransitions.ContainsKey(current) ||
                        !validTransitions[current].Contains(next))
                    {
                        result.Errors.Add(new
                        {
                            Field = "Status",
                            Message = $"Invalid status transition from {current} to {next}"
                        });
                    }

                    // assigned_to check: if status is changing to in_progress, dto.assigned_to cannot be null
                    if(next == "in_progress" && string.IsNullOrEmpty(dto.AssignedTo))
                    {
                        result.Errors.Add(new
                        {
                            Field = "AssignedTo",
                            Message = "AssignedTo is required when status is changed to in_progress"
                        });
                    }

                    // assigned_to check: if status is changing to done, assigned_to cannot be changing
                    if(next == "done" && !string.IsNullOrEmpty(dto.AssignedTo) && dto.AssignedTo != task.AssignedTo)
                    {
                        result.Errors.Add(new
                        {
                            Field = "AssignedTo",
                            Message = "AssignedTo cannot be changed when status is changed to done"
                        });
                    }

                    // create TaskHistoryDTO
                    var history = new CreateTaskHistoryDTO
                    {
                        TaskId = dto.Id,
                        FieldChanged = "status",
                        OldValue = task.Status,
                        NewValue = dto.Status,
                        ChangedBy = task.CreatedBy
                    };

                    // add to historyChanges list
                    historyChanges.Add(history);
                }

                // AssignedTo check
                if (!string.IsNullOrEmpty(dto.AssignedTo) && dto.AssignedTo != task.AssignedTo)
                {
                    // create TaskHistoryDTO
                    var history = new CreateTaskHistoryDTO
                    {
                        TaskId = dto.Id,
                        FieldChanged = "assigned_to",
                        OldValue = task.AssignedTo?.ToString(),
                        NewValue = dto.AssignedTo?.ToString(),
                        ChangedBy = task.CreatedBy
                    };

                    // add to historyChanges list
                    historyChanges.Add(history);
                }

                if (result.Errors.Count > 0)
                {
                    result.Data = null;
                    result.StatusCode = "400";
                    return result;
                }

                if (dto.Title != null) task.Title = dto.Title;
                if (dto.Description != null) task.Description = dto.Description;
                if (dto.Status != null) task.Status = dto.Status;
                if (dto.Priority != null) task.Priority = dto.Priority;
                if (dto.DueDate.HasValue) task.DueDate = dto.DueDate;

                // save history changes
                foreach (var change in historyChanges)
                {
                    _history.AddHistory(change);
                }

                _repo.Update(task);
                _repo.Save();

                result.Data = new TasksDTO
                {
                    Id = task.Id,
                    Title = task.Title,
                    Status = task.Status,
                    Priority = task.Priority,
                    AssignedUserName = task.AssignedToNavigation?.UserName,
                    DueDate = task.DueDate
                };

                result.StatusCode = "200";

                return result;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Errors.Add(new { Exception = ex.Message });
                result.StatusCode = "500";
                return result;
            }
        }

        public ApiResponse DeleteTask(int id)
        {
            var result = new ApiResponse();
            try
            {
                // Task must exist
                var task = _repo.GetById(id);
                if (task == null)
                {
                    result.Errors.Add(new { Message = "Task not found" });
                    result.StatusCode = "404";
                    return result;
                }

                _repo.Delete(task);
                _repo.Save();

                result.StatusCode = "200";

                return result;
            }
            catch (Exception ex)
            {
                result.Data = null;
                result.Errors.Add(new { Exception = ex.Message });
                result.StatusCode = "500";
                return result;
            }
        }

        public ApiResponse AssignTask(AssignTaskDTO dto)
        {
            var result = new ApiResponse();

            var userId = _httpContext.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // check task exists
            var task = _repo.GetById(dto.TaskId);
            if (task == null)
            {
                result.StatusCode = "404";
                result.Errors.Add(new { Message = "Task not found" });
                return result;
            }

            // check current user is member in project
            var currentMember = _projectMemberRepo
                .GetByUserAndProject(userId, (int)task.ProjectId);

            if (currentMember == null)
            {
                result.StatusCode = "403";
                result.Errors.Add(new { Message = "Not part of project" });
                return result;
            }

            // check permission (only PM or Admin)
            var userRole = _httpContext.HttpContext.User.FindFirstValue(ClaimTypes.Role);

            if (userRole != "Admin" && currentMember.Role != "ProjectManager")
            {
                result.StatusCode = "403";
                result.Errors.Add(new { Message = "Not allowed to assign tasks" });
                return result;
            }

            // check assigned user is member
            var assignedMember = _projectMemberRepo
                .GetByUserAndProject(dto.AssignedTo, (int)task.ProjectId);

            if (assignedMember == null)
            {
                result.StatusCode = "400";
                result.Errors.Add(new { Message = "User not in project" });
                return result;
            }

            //  assign
            var oldAssigned = task.AssignedTo;
            task.AssignedTo = dto.AssignedTo;

            // auto move status
            if (task.Status == "todo")
                task.Status = "in_progress";

            // add history
            _history.AddHistory(new CreateTaskHistoryDTO
            {
                TaskId = task.Id,
                FieldChanged = "assigned_to",
                OldValue = oldAssigned,
                NewValue = dto.AssignedTo,
                ChangedBy = userId
            });

            _repo.Update(task);
            _repo.Save();

            result.StatusCode = "200";
            result.Data = task;

            return result;
        }
    }
}
