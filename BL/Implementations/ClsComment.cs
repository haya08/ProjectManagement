using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Comments;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.BL.Implementations
{
    public class ClsComment : IComment
    {
        private readonly ICommentRepository _repo;

        public ClsComment(ICommentRepository repo)
        {
            _repo = repo;
        }

        public ApiResponse GetByTaskId(int taskId)
        {
            var comments = _repo.GetByTaskId(taskId);

            var result = comments.Select(c => new CommentDTO
            {
                Id = c.Id,
                Content = c.Content,
                UserName = c.User?.UserName,
                CreatedAt = c.CreatedAt
            });

            return new ApiResponse
            {
                Data = result,
                StatusCode = "200"
            };
        }

        public ApiResponse AddComment(int taskId, string userId, CreateCommentDTO dto)
        {
            var result = new ApiResponse();

            if (string.IsNullOrWhiteSpace(dto.Content))
            {
                result.Errors.Add(new { Field = "Content", Message = "Content is required" });
                result.StatusCode = "400";
                return result;
            }

            var comment = new TbComment
            {
                TaskId = taskId,
                UserId = userId,
                Content = dto.Content,
                CreatedAt = DateTime.UtcNow
            };

            _repo.Add(comment);
            _repo.Save();

            result.Data = comment;
            result.StatusCode = "201";
            return result;
        }

        public ApiResponse DeleteComment(int commentId, string userId, string role)
        {
            var result = new ApiResponse();

            var comment = _repo.GetById(commentId);

            if (comment == null)
            {
                result.Errors.Add(new { Message = "Comment not found" });
                result.StatusCode = "404";
                return result;
            }

            // 🔥 authorization
            if (role != "Admin" && comment.UserId != userId)
            {
                result.Errors.Add(new { Message = "Not allowed" });
                result.StatusCode = "403";
                return result;
            }

            _repo.Delete(comment);
            _repo.Save();

            result.StatusCode = "200";
            return result;
        }
    }
}
