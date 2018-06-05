
using System.Configuration;

namespace quiniela.Models
{
    public abstract class BaseClass
    {
        public string OwnerEmail { get { return ConfigurationManager.AppSettings["ownerEmail"]; } }
    }
}