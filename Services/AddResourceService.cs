using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;
using MSProj_Analog.Interfaces;
using Xceed.Wpf.AvalonDock.Properties;

namespace MSProj_Analog.Services
{
    public class AddResourceService : IAddResourceService
    {
        public void AddResource(ICollection<Resource> resources, AppDbContext context, Resource resource)
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

