﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebApiTest.Intefaces;

namespace WebApiTest.Models
{
    public class BaseApiModel:IBaseApiModel
    {
        public int Id { get; set; }
    }
}