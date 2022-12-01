using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace OpenXML.Abstractions.Models
{
    [DataContract]
    public class BaseQueryModel : IQueryModel
    {
        private Guid _id;
        protected bool _isDeleted;
        public BaseQueryModel()
        {
        }
        protected BaseQueryModel(Guid id, bool isDeleted)
        {
            if (Equals(id, default(Guid)))
            {
                throw new ArgumentException("The ID cannot be the default value.", "id");
            }

            this._id = id;
            this._isDeleted = isDeleted;
        }

        [DataMember]
        public Guid Id
        {
            get => this._id;
            set => _id = value;

        }

        [JsonIgnore]
        public bool IsDeleted
        {
            get => this._isDeleted;
            set => _isDeleted = value;
        }

        public void SetAsDeleted()
        {
            _isDeleted = true;
        }
    }
}
