namespace Dior.Library.BO.UserInterface
{
    public class User
    {
        public int Id { get; set; } // Un seul identifiant
        public string UserName { get; set; }
        public Boolean IsActive { get; set; }
        public String LastName { get; set; }
        public String FirstName { get; set; }
        public String LastEditBy { get; set; }
        public DateTime LastEditAt { get; set; }
        public String Email { get; set; }
        public String Phone { get; set; }
    }
}
