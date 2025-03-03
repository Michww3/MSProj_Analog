using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using MSProj_Analog.Interfaces;

namespace MSProj_Analog.Services
{
    public class AddResourceService : IAddResourceService
    {
        public void AddResource(AppDbContext context, ICollection<Resource> resources, Resource resource)
        {
            resources.Add(resource);
            using (context)
            {
                context.Resources.Add(resource);
                context.SaveChanges();
            }
        }
    }
}