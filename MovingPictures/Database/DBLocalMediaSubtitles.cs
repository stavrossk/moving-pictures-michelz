using System;
using System.Collections.Generic;
using System.IO;
using Cornerstone.Extensions;
using Cornerstone.Extensions.IO;
using Cornerstone.Database;
using Cornerstone.Database.CustomTypes;
using Cornerstone.Database.Tables;
using MediaPortal.Plugins.MovingPictures.LocalMediaManagement;
using NLog;

namespace MediaPortal.Plugins.MovingPictures.Database {
    [DBTableAttribute("local_media_subtitles")]
    public class DBLocalMediaSubtitles : MovingPicturesDBTable {

        private static Logger logger = LogManager.GetCurrentClassLogger();

        private bool deleting = false;

        public override void AfterDelete() {
        }

        #region read-only properties

        #endregion

        #region Database Fields

        [DBFieldAttribute(Filterable = false)]
        public DBLocalMedia LocalMedia
        {
            get { return _localMedia; }
            set
            {
                _localMedia = value;
                commitNeeded = true;
            }
        } private DBLocalMedia _localMedia;

        /// <summary>
        /// Language
        /// </summary>
        [DBFieldAttribute]
        public string Language
        {
            get { return _language; }
            set
            {
              _language = value;
                commitNeeded = true;
            }
        } private string _language;

        /// <summary>
        /// Internal (Text Stream) or External (File)
        /// </summary>
        [DBFieldAttribute]
        public bool Internal
        {
          get { return _internal; }
          set
          {
            _internal = value;
            commitNeeded = true;
          }
        } private bool _internal;

        #endregion


        #region Public methods



        #endregion

        #region Overrides

        public override bool Equals(object obj) {
            // make sure we have a DBLocalMediaSubtitles object
            if (obj == null || obj.GetType() != typeof(DBLocalMediaSubtitles))
                return false;

            // make sure both objects are valid
            DBLocalMediaSubtitles otherLocalMedia = (DBLocalMediaSubtitles)obj;
            if (otherLocalMedia.LocalMedia == null || this.LocalMedia == null)
                return false;

            // make sure we have the same language
            if (!this.LocalMedia.Equals(otherLocalMedia.LocalMedia) && !this.Language.Equals(otherLocalMedia.Language))
                return false;

            // make sure we are both internal
            if (!this.Internal.Equals(otherLocalMedia.Internal))
              return false;

           return true;
        }

        public override int GetHashCode() {
            if (Language != null)
                return (LocalMedia + "|" + Language + "|" + Internal).GetHashCode();

            return base.GetHashCode();
        }

        public override string ToString() {
            if (Language != null)
                return Language + ", internal: " + Internal;

            return base.ToString();
        }
        #endregion

        #region Database Management Methods

        /// <summary>
        /// Delete the Subtitle
        /// </summary>
        public override void Delete() {
            if (deleting)
                return;

            deleting = true;
            logger.Info("Removing Subtitle with ID: {0}", ID);

            base.Delete();
            deleting = false;
        }

        /// <summary>
        /// Delete All Subtitles for a <paramref name="localMediaId"/>
        /// </summary>
        /// <param name="localMediaId"></param>
        public static void DeleteAllForLocalMedia(int localMediaId)
        {
            if (!(localMediaId >= 0))
            {
                logger.Error("Cannot remove Subtitles because LocalMedia ID is invalid: {0}", localMediaId);
                return;
            }

            logger.Info("Removing all Subtitles for LocalMedia with ID: {0}", localMediaId);
            foreach (var stream in GetAll(localMediaId))
            {
                stream.Delete();
            }

        }

        /// <summary>
        /// Get Subtitle by <paramref name="dbId"/>
        /// </summary>
        /// <param name="dbId">Subtitle Database ID</param>
        /// <returns></returns>
        public static DBLocalMediaSubtitles Get(int dbId)
        {
            return MovingPicturesCore.DatabaseManager.Get<DBLocalMediaSubtitles>(dbId);
        }

        /// <summary>
        /// Get Subtitles by <paramref name="localMediaId"/> and <paramref name="language"/> and <paramref name="isInternal"/>
        /// </summary>
        /// <param name="localMediaId">Local Media Id</param>
        /// <param name="language">Language</param>
        /// <param name="isInternal">Are there internal subtitles?</param>
        /// <returns></returns>
        public static DBLocalMediaSubtitles Get(int localMediaId, string language, bool isInternal)
        {
            logger.Debug("localMediaId: {0} | language: {1} | isInternal: {2}");
            
            logger.Debug("Constructing LocalMedia Criteria...");
            DBField localMediaField = DBField.GetField(typeof(DBLocalMediaSubtitles), "LocalMedia");
            ICriteria localMediaCriteria = new BaseCriteria(localMediaField, "=", localMediaId);

            ICriteria localMediaAndLanguageCriteria = localMediaCriteria;

            logger.Debug("Constructing language Criteria...");
            DBField languageField = DBField.GetField(typeof(DBLocalMediaSubtitles), "Language");
            ICriteria languageCriteria = new BaseCriteria(languageField, "=", language);
            localMediaAndLanguageCriteria = new GroupedCriteria(localMediaCriteria, GroupedCriteria.Operator.AND, languageCriteria);

            ICriteria criteria = localMediaAndLanguageCriteria;

            logger.Debug("Constructing Internal Criteria...");
            DBField isInternalField = DBField.GetField(typeof(DBLocalMediaSubtitles), "Internal");
            ICriteria internalCriteria = new BaseCriteria(isInternalField, "=", isInternal);
            criteria = new GroupedCriteria(localMediaAndLanguageCriteria, GroupedCriteria.Operator.AND, internalCriteria);

            logger.Debug("Getting resultSet...");
            List<DBLocalMediaSubtitles> resultSet = MovingPicturesCore.DatabaseManager.Get<DBLocalMediaSubtitles>(criteria);
            return resultSet.Count > 0 ? resultSet[0] : null;
        }

        /// <summary>
        /// Get all Subtitles Streams for LocalMedia with <paramref name="localMediaId"/>
        /// </summary>
        /// <param name="localMediaId">LocalMedia ID</param>
        /// <returns></returns>
        public static List<DBLocalMediaSubtitles> GetAll(int localMediaId)
        {
            DBField localMediaField = DBField.GetField(typeof(DBLocalMediaSubtitles), "LocalMedia");
            ICriteria localMediaCriteria = new BaseCriteria(localMediaField, "=", localMediaId);

            List<DBLocalMediaSubtitles> resultSet = MovingPicturesCore.DatabaseManager.Get<DBLocalMediaSubtitles>(localMediaCriteria);
            return resultSet;
        }

        public static List<DBLocalMediaSubtitles> GetAll()
        {
            return MovingPicturesCore.DatabaseManager.Get<DBLocalMediaSubtitles>(null);
        }

        #endregion
    }


}
