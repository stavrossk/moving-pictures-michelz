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
    [DBTableAttribute("local_media_audio_streams")]
    public class DBLocalMediaAudioStreams : MovingPicturesDBTable {

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
        /// Audio Stream ID
        /// </summary>
        [DBFieldAttribute(Filterable = false)]
        public int AudioStreamId
        {
            get { return _audioStreamId; }
            set
            {
                _audioStreamId = value;
                commitNeeded = true;
            }
        } private int _audioStreamId;

        /// <summary>
        /// Audio Codec
        /// </summary>
        [DBFieldAttribute(AllowManualFilterInput = false)]
        public string AudioCodec {
            get { return _audioCodec; }
            set {
                _audioCodec = value;
                commitNeeded = true;
            }
        } private string _audioCodec;

        /// <summary>
        /// Audio Channels (Friendly)
        /// </summary>
        [DBFieldAttribute]
        public string AudioChannels {
            get { return _audioChannels; }
            set {
                _audioChannels = value;
                commitNeeded = true;
            }
        } private string _audioChannels;

        /// <summary>
        /// Audio Language (2-Letter ISO 3166-1 Code)
        /// </summary>
        [DBFieldAttribute]
        public string AudioLanguage
        {
            get { return _audioLanguage; }
            set
            {
                _audioLanguage = value;
                commitNeeded = true;
            }
        } private string _audioLanguage;

        /// <summary>
        /// Is the Audio Stream a Commentary Stream?
        /// </summary>
        [DBFieldAttribute]
        public bool AudioIsCommentary
        {
          get { return _audioIsCommentary; }
          set
          {
            _audioIsCommentary = value;
            commitNeeded = true;
          }
        } private bool _audioIsCommentary;

        #endregion

        //#region Database Relations
        //[DBRelation(AutoRetrieve = true)]
        //public RelationList<DBLocalMediaAudioStreams, DBLocalMedia> LocalMedia
        //{
        //    get
        //    {
        //        if (_localMedia == null)
        //        {
        //            _localMedia = new RelationList<DBLocalMediaAudioStreams, DBLocalMedia>(this);
        //        }
        //        return _localMedia;
        //    }
        //} RelationList<DBLocalMediaAudioStreams, DBLocalMedia> _localMedia;

        //#endregion

        #region Public methods



        #endregion

        #region Overrides

        public override bool Equals(object obj) {
            // make sure we have a dblocalmediaaudiostreams object
            if (obj == null || obj.GetType() != typeof(DBLocalMediaAudioStreams))
                return false;

            // make sure both objects are valid
            DBLocalMediaAudioStreams otherLocalMedia = (DBLocalMediaAudioStreams)obj;
            if (otherLocalMedia.LocalMedia == null || this.LocalMedia == null)
                return false;

            // make sure we have the same stream
            if (!this.LocalMedia.Equals(otherLocalMedia.LocalMedia) && !this.AudioStreamId.Equals(otherLocalMedia.AudioStreamId))
                return false;

           return true;
        }

        public override int GetHashCode() {
            if (AudioChannels != null)
                return (LocalMedia + "|" + AudioStreamId).GetHashCode();

            return base.GetHashCode();
        }

        public override string ToString() {
            if (AudioChannels != null)
                return AudioLanguage + ": " + AudioChannels + " " + AudioCodec;

            return base.ToString();
        }
        #endregion

        #region Database Management Methods

        /// <summary>
        /// Delete the AudioStream
        /// </summary>
        public override void Delete() {
            if (deleting)
                return;

            deleting = true;
            logger.Info("Removing AudioStream with ID: {0}", ID);

            base.Delete();
            deleting = false;
        }

        /// <summary>
        /// Delete All AudioStreams for a <paramref name="localMediaId"/>
        /// </summary>
        /// <param name="localMediaId"></param>
        public static void DeleteAllForLocalMedia(int localMediaId)
        {
            if (!(localMediaId >= 0))
            {
                logger.Error("Cannot remove AudioStreams because LocalMedia ID is invalid: {0}", localMediaId);
                return;
            }

            logger.Info("Removing all AudioStreams for LocalMedia with ID: {0}", localMediaId);
            foreach (var stream in GetAll(localMediaId))
            {
                stream.Delete();
            }

        }

        /// <summary>
        /// Get Audio Stream by <paramref name="dbId"/>
        /// </summary>
        /// <param name="dbId">Audio Stream Database ID</param>
        /// <returns></returns>
        public static DBLocalMediaAudioStreams Get(int dbId)
        {
            return MovingPicturesCore.DatabaseManager.Get<DBLocalMediaAudioStreams>(dbId);
        }

        /// <summary>
        /// Get Audio Stream by <paramref name="localMediaId"/> and <paramref name="audioStreamId"/>
        /// </summary>
        /// <param name="localMediaId">Local Media Id</param>
        /// <param name="audioStreamId">Audio Stream ID</param>
        /// <returns></returns>
        public static DBLocalMediaAudioStreams Get(int localMediaId, int audioStreamId)
        {
            DBField localMediaField = DBField.GetField(typeof(DBLocalMediaAudioStreams), "LocalMedia");
            ICriteria localMediaCriteria = new BaseCriteria(localMediaField, "=", localMediaId);

            ICriteria criteria = localMediaCriteria;

            DBField audioStreamIdField = DBField.GetField(typeof(DBLocalMediaAudioStreams), "AudioStreamId");
            ICriteria audioStreamIdCriteria = new BaseCriteria(audioStreamIdField, "=", audioStreamId);
            criteria = new GroupedCriteria(localMediaCriteria, GroupedCriteria.Operator.AND, audioStreamIdCriteria);

            List<DBLocalMediaAudioStreams> resultSet = MovingPicturesCore.DatabaseManager.Get<DBLocalMediaAudioStreams>(criteria);
            return resultSet.Count > 0 ? resultSet[0] : null;
        }

        /// <summary>
        /// Get all Audio Streams for LocalMedia with <paramref name="localMediaId"/>
        /// </summary>
        /// <param name="localMediaId">LocalMedia ID</param>
        /// <returns></returns>
        public static List<DBLocalMediaAudioStreams> GetAll(int localMediaId)
        {
            DBField localMediaField = DBField.GetField(typeof(DBLocalMediaAudioStreams), "LocalMedia");
            ICriteria localMediaCriteria = new BaseCriteria(localMediaField, "=", localMediaId);

            List<DBLocalMediaAudioStreams> resultSet = MovingPicturesCore.DatabaseManager.Get<DBLocalMediaAudioStreams>(localMediaCriteria);
            return resultSet;
        }

        public static List<DBLocalMediaAudioStreams> GetAll() {
            return MovingPicturesCore.DatabaseManager.Get<DBLocalMediaAudioStreams>(null);
        }

        #endregion
    }


}
