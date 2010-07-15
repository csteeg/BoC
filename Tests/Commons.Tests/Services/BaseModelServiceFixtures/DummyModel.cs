using BoC.Persistence;

namespace BoC.Tests.Services.BaseModelServiceFixtures
{
    public class DummyModel: IBaseEntity
    {
        public bool Equals(IBaseEntity other)
        {
            return other == this;
        }

        public object Id { get; set; }

        public bool Saved { get; set; }
    }
}