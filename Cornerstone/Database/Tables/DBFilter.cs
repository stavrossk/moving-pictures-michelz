﻿using System;
using System.Collections.Generic;
using System.Text;
using Cornerstone.Database.CustomTypes;

namespace Cornerstone.Database.Tables {
    [DBTableAttribute("filters")]
    public class DBFilter<T> : GenericDatabaseTable<T>, IFilter<T>, IDBFilter, IGenericFilter
        where T : DatabaseTable {
        
        public enum CriteriaGroupingEnum {
            ALL,
            ONE,
            NONE
        }
        
        #region IFilter<T> Members

        public event FilterUpdatedDelegate<T> Updated;

        public HashSet<T> Filter(ICollection<T> input) {
            return Filter(input, false);
        }

        public HashSet<T> Filter(ICollection<T> input, bool forceActive) {
            bool active = forceActive || _active;
            HashSet<T> results = new HashSet<T>();

            // if we are not active, or the filter has no inclusive rules start
            // with everything and remove the blacklist
            if (!active || (Criteria.Count == 0 && WhiteList.Count == 0)) {
                if (!active && input is HashSet<T>)
                    return input as HashSet<T>;
                
                foreach (T currItem in input)
                    results.Add(currItem);

                if (!active) return results;

                // remove blacklist items
                if (active)
                    foreach (T currItem in BlackList) {
                        if (BlackList.Contains(currItem))
                            results.Remove(currItem);
                    }

                return CheckInversion(input, results);
            }


            // if there is no criteria and no blacklisted items, just use the white list
            if (Criteria.Count == 0 && BlackList.Count == 0) {
                foreach (T currItem in WhiteList)
                    if (input.Contains(currItem))
                        results.Add(currItem);
                return CheckInversion(input, results);
            }

            // handle AND type criteria
            bool first = true;
            if (CriteriaGrouping == CriteriaGroupingEnum.ALL)
                foreach (DBCriteria<T> currCriteria in Criteria) {
                    results = currCriteria.Filter(first ? input : results, active);
                    first = false;
                }

            // handle OR type criteria
            if (CriteriaGrouping == CriteriaGroupingEnum.ONE) {
                HashSet<T> okItems = new HashSet<T>();
                foreach (DBCriteria<T> currCriteria in Criteria) {
                    HashSet<T> tmp = currCriteria.Filter(input, active);
                    okItems.UnionWith(tmp);
                }

                results = okItems;
            }

            // handle NONE type criteria
            if (CriteriaGrouping == CriteriaGroupingEnum.NONE) {
                foreach (T currItem in input)
                    results.Add(currItem);

                HashSet<T> excludeItems = new HashSet<T>();
                foreach (T currItem in input)
                    excludeItems.Add(currItem);

                foreach (DBCriteria<T> currCriteria in Criteria)
                    excludeItems = currCriteria.Filter(excludeItems, active);

                foreach(T item in excludeItems)
                    results.Remove(item);   
            }

            // remove blacklist items
            foreach (T currItem in BlackList) {
                if (BlackList.Contains(currItem))
                    results.Remove(currItem);
            }

            // make sure all whitelist items are in the result list
            foreach (T item in WhiteList)
                if (!results.Contains(item) && input.Contains(item))
                    results.Add(item);

            return CheckInversion(input, results);
        }

        private HashSet<T> CheckInversion(ICollection<T> input, HashSet<T> filtered) {
            if (!Invert) return filtered;

            HashSet<T> output = new HashSet<T>();
            foreach (T currItem in input)
                if (!filtered.Contains(currItem))
                    output.Add(currItem);

            return output;
        }

        public bool Active {
            get { return _active; }
            set {
                if (_active != value) {
                    _active = value;

                    if (Updated != null)
                        Updated(this);
                }
            }
        }
        private bool _active = true;

        public bool Invert {
            get { return _invert; }
            set {
                _invert = value;

                if (Updated != null)
                    Updated(this);
            }
        } private bool _invert = false;

        #endregion

        #region Database Fields
        
        [DBField]
        public string Name {
            get { return _name; }
            set {
                _name = value;
                commitNeeded = true;
            }
        } private string _name;

        [DBField]
        public CriteriaGroupingEnum CriteriaGrouping {
            get { return _criteriaGrouping; }

            set {
                _criteriaGrouping = value;
                commitNeeded = true;
            }
        } private CriteriaGroupingEnum _criteriaGrouping;

        [DBRelation(AutoRetrieve = true)]
        public RelationList<DBFilter<T>, DBCriteria<T>> Criteria {
            get {
                if (_criteria == null) {
                    _criteria = new RelationList<DBFilter<T>,DBCriteria<T>>(this);
                }
                return _criteria;
            }
        } RelationList<DBFilter<T>, DBCriteria<T>> _criteria;

        [DBRelation(AutoRetrieve = true, Identifier="white_list")]
        public RelationList<DBFilter<T>, T> WhiteList {
            get {
                if (_whiteList == null) {
                    _whiteList = new RelationList<DBFilter<T>, T>(this);
                }
                return _whiteList;
            }
        } RelationList<DBFilter<T>, T> _whiteList;

        [DBRelation(AutoRetrieve = true, Identifier="black_list")]
        public RelationList<DBFilter<T>, T> BlackList {
            get {
                if (_blackList == null) {
                    _blackList = new RelationList<DBFilter<T>, T>(this);
                }
                return _blackList;
            }
        } RelationList<DBFilter<T>, T> _blackList;


        #endregion

        public override void Delete() {
            base.Delete();

            foreach (DBCriteria<T> currCriteria in Criteria)
                currCriteria.Delete();
        }

        public override string ToString() {
            return Name;
        }
    }

    // empty interface to handle DBFilters generically
    public interface IDBFilter {}

    // empty interface to handle DBFilters generically
    public interface IGenericFilter { }
}
