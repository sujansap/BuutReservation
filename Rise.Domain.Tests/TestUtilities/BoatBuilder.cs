using Rise.Domain.Boats;
using Rise.Domain.Common;
using Rise.Domain;

namespace Rise.Domain.Tests.TestUtilities
{
    public class BoatBuilder
    {
        public const string ValidPersonalName = "Limba";

        private string personalName = ValidPersonalName;
        private int id;

        public BoatBuilder WithPersonalName(string personalName)
        {
            this.personalName = personalName;
            return this;
        }

        public BoatBuilder WithId(int id)
        {
            this.id = id;
            return this;
        }

        public Boat Build()
        {
            var boat = new Boat
            {
                PersonalName = personalName
            };
            
            // Use reflection to set the protected Id property for testing purposes
            typeof(Boat)
                .BaseType!
                .GetProperty("Id")!
                .SetValue(boat, id);

            return boat;
        }

    }

}

