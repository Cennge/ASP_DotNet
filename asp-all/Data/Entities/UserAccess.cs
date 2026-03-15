namespace asp_all.Data.Entities
{
    public class UserAccess
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid UserRoleId { get; set; }

        public String Login { get; set; } = null!;

        public String Salt { get; set; } = null!;

        public String Dk { get; set; } = null!;

        public DateTime CreatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        //навігаційні влстивості - посилнная на ішні сутності - спрощена схема зв'язування даних
        public UserData UserData { get; set; } = null!;

        public UserRole UserRole { get; set; } = null!;

    }
}
