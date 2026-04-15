using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Projects;

namespace ProjectManagement.BL.Interfaces
{
    public interface IProject
    {
        ApiResponse GetAll();
        ApiResponse GetById(int id);
        ApiResponse Create(CreateProjectDTO dto);
        ApiResponse Update(UpdateProjectDTO dto);
        ApiResponse Delete(int id);
    }
}
