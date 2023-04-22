using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PATHSMap.Data;
using PATHSMap;

public partial class MyPage
{
    [Inject]
    public IConfiguration Configuration { get; set; }
}