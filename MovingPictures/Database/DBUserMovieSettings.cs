using System;
using System.Collections.Generic;
using System.Text;
using Cornerstone.Database;
using Cornerstone.Database.CustomTypes;
using Cornerstone.Database.Tables;

namespace MediaPortal.Plugins.MovingPictures.Database {
    [DBTableAttribute("user_movie_settings")]
    public class DBUserMovieSettings: MovingPicturesDBTable {
        [Obsolete]
        public bool RatingChanged = false;

        [Obsolete]
        public bool WatchCountChanged = false;

        public override void AfterDelete() {
        }

        #region Database Fields

        [DBFieldAttribute(Filterable=false)]
        public DBUser User {
            get { return user; }
            set {
                user = value;
                commitNeeded = true;
                FieldChanged("User");
            }
        } private DBUser user;

        // Value between 0 and 10
        [DBFieldAttribute(FieldName = "user_rating", Default = null, AllowDynamicFiltering=false)]
        public int? UserRating {
            get { return _userRating; }
            set {
                if (value > 5) value = 5;
                if (value < 1) value = 1;

                if (_userRating != value) {
                    _userRating = value;
                    commitNeeded = true;
                    FieldChanged("UserRating");
                }
            }
        } private int? _userRating;

        [DBFieldAttribute(FieldName = "watched", AllowDynamicFiltering=false)]
        public int WatchedCount {
            get { return _watched; }
            set {
                if (_watched != value) {
                    _watched = value;
                    commitNeeded = true;
                    FieldChanged("WatchedCount");
                }
            }
        } private int _watched;

        [DBFieldAttribute(FieldName = "resume_part", Filterable = false)]
        public int ResumePart {
            get { return _resumePart; }

            set {
                _resumePart = value;
                commitNeeded = true;
                FieldChanged("ResumePart");
            }
        } private int _resumePart;


        [DBFieldAttribute(FieldName = "resume_time")]
        public int ResumeTime {
            get { return _resumeTime; }

            set {
                _resumeTime = value;
                commitNeeded = true;
                FieldChanged("ResumeTime");
            }
        } private int _resumeTime;

        [DBFieldAttribute(FieldName = "resume_data", Default = null, Filterable = false)]
        public ByteArray ResumeData {
            get { return _resumeData; }

            set {
                _resumeData = value;
                commitNeeded = true;
                FieldChanged("ResumeData");
            }
        } private ByteArray _resumeData;

        [DBRelation(AutoRetrieve = true, Filterable = false)]
        public RelationList<DBUserMovieSettings, DBMovieInfo> AttachedMovies {
            get {
                if (_attachedMovies == null) {
                    _attachedMovies = new RelationList<DBUserMovieSettings, DBMovieInfo>(this);
                }
                return _attachedMovies;
            }
        } RelationList<DBUserMovieSettings, DBMovieInfo> _attachedMovies;

        #endregion

        #region Database Management Methods
   
        public static DBUserMovieSettings Get(int id) {
            return MovingPicturesCore.DatabaseManager.Get<DBUserMovieSettings>(id);
        }

        public static List<DBUserMovieSettings> GetAll() {
            return MovingPicturesCore.DatabaseManager.Get<DBUserMovieSettings>(null);
        }

        #endregion
    }
}
