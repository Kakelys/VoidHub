using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForumApi.DTO.Filter;

public class Criteria
{
    public string Name { get; set; }
    public string Op { get; set; }
    public object Value { get; set; }
}
