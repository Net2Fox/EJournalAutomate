using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EJournalWPF.Model
{
    internal class CacheData
    {
        public List<Student> Students;
        public List<Group> Groups;

        public CacheData(List<Student> students, List<Group> groups)
        {
            this.Students = students;
            this.Groups = groups;
        }
    }
}
