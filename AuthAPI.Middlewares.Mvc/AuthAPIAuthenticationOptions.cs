﻿using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Text;

namespace AuthAPI.Middlewares.Mvc
{
    public class AuthAPIAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "AuthAPI";
        public string Scheme => DefaultScheme;
    }
}
