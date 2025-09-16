namespace EterationCaseStudy.Domain.Entities.Base
{
    public class Entity
    {
        public int Id { get; set; }

        public DateTime CreatedDate { get; set; }

        public int? CreatedById { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public int? ModifiedById { get; set; }

        public bool IsDeleted { get; set; }
    }
}
