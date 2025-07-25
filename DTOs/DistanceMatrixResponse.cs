namespace ECS_Logistics.DTOs;

public class DistanceMatrixResponse
{
    public List<Row> rows { get; set; }
    public List<string> destination_addresses { get; set; }
    public List<string> origin_addresses { get; set; }
    public string status { get; set; }
}

public class Row
{
    public List<Element> elements { get; set; }
}

public class Element
{
    public Distance distance { get; set; }
    public Duration duration { get; set; }
    public string status { get; set; }
}

public class Distance
{
    public string text { get; set; } // e.g., "45.2 km"
    public int value { get; set; }   // in meters
}

public class Duration
{
    public string text { get; set; } // e.g., "36 mins"
    public int value { get; set; }   // in seconds
}