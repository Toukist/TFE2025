

namespace Dior.Library.DTO
{
    public class PrivilegeDto
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsConfigurableRead { get; set; }
        public bool IsConfigurableDelete { get; set; }
        public bool IsConfigurableAdd { get; set; }
        public bool IsConfigurableModify { get; set; }
        public bool IsConfigurableStatus { get; set; }
        public bool IsConfigurableExecution { get; set; }
        public string LastEditBy { get; set; }
        public DateTime LastEditAt { get; set; }
    }

}
