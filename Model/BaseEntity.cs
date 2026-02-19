using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    // Base class for all entities in the system - provides common ID property
    [DataContract]
    public class BaseEntity
    {
        // Field: Unique identifier for the entity
        private int id;

        // Constructor: Creates a new base entity
        public BaseEntity() { }

        // Property: Gets or sets the entity's unique ID
        [DataMember]
        public int Id
        {
            // Return the ID value
            get { return id; }
            // Set the ID value
            set { id = value; }
        }
    }
}