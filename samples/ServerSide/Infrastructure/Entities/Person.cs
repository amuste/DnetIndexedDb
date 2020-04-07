namespace DnetIndexedDbServer.Infrastructure.Entities
{
    public class Person
    {
        public string Id {get; set; }

        public int? Index { get; set; }

        public bool IsActive { get; set; } = false;

        public string Balance { get; set; }

        public string Picture { get; set; }

        public int? Age { get; set; }

        public string EyeColor { get; set; }

        public string Name { get; set; }

        public string Gender { get; set; }

        public string Company { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string Address { get; set; }

        public string Registered { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public string Greeting { get; set; }

        public string FavoriteFruit { get; set; }
    }
}
