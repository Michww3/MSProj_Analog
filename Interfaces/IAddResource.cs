﻿using MSProj_Analog.DTOs;
using MSProj_Analog.Helpers;

namespace MSProj_Analog.Interfaces
{
    public interface IAddResource
    {
        public void AddResource(ICollection<Resource> resources,AppDbContext context, Resource res);

    }
}
