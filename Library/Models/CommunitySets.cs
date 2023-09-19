﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manlaan.CommanderMarkers.Library.Models;

public  class CommunitySets
{

    [JsonProperty("lastEdit")]
    public DateTime LastEdit { get; set; } = DateTime.UtcNow;

    [JsonProperty("categories")]
    public List<CommunityCategory> Categories { get; set; } = new();

}
