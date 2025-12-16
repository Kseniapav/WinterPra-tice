using System;

namespace RepairRequests
{
    public class Model
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Problem { get; set; }
        public DateTime Deadline { get; set; }
        public string Responsible { get; set; }
        public string Status { get; set; }

    }
}
