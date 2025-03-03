using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;

namespace MSProj_Analog.Interfaces
{
    public interface IAddResourceService
    {
        public void AddResource(AppDbContext context, ICollection<Resource> resources, Resource res);
    }
}
