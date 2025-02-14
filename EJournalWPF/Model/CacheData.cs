using EJournalWPF.Model.API.MessageModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalWPF.Model
{
    internal class CacheData
    {
        public List<Group> Groups;

        public CacheData(List<Group> groups)
        {
            this.Groups = groups;
        }
    }
}
