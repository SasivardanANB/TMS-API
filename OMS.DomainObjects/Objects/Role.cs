﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace OMS.DomainObjects.Objects
{
    public class Role
    {
        public int ID { get; set; }
        [Required(ErrorMessageResourceType = typeof(Resource.ResourceData), ErrorMessageResourceName = "InvalidRoleCode")]
        [MaxLength(4)]
        public string RoleCode { get; set; }
        [MaxLength(30)]
        public string RoleDescription { get; set; }
        public bool IsActive { get; set; }
        public List<RoleMenu> RoleMenus { get; set; }
    }
}
