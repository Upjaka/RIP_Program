using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaApplication2.Models
{
    public class ChangeList
    {
        private List<LocalChange> localChanges;

        public int Count
        {
            get { return localChanges.Count; }
        }

        public ChangeList()
        {
            localChanges = new List<LocalChange>();
        }

        public void Add(LocalChange change)
        {
            if (change.IsInsert)
            {
                localChanges.Add(change);
            }
            else
            {
                foreach (var localChange in localChanges)
                {
                    if (localChange.HasSameCar(change))
                    {
                        localChange.SetNewCar(change);
                        break;
                    }
                }
            }            
        }

        public List<string> GetSQLRequests()
        {
            var result = new List<string>();
            foreach (var change in localChanges)
            {
                result.Add(change.GetSQL());
            }
            return result;
        }

        public void Clear()
        {
            localChanges.Clear();
        }
    }
}
